using V83;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace _1sServerWidget
{
    internal class ConnectToAgent : IDisposable
    {
        private UpdateStateEvents _updateStateEvents;
        private UpdateSessionsInfoEvents _updateSessionsInfoEvents;

        private readonly string _serverName;
        private COMConnector _comConnector;
        private IServerAgentConnection _serverAgent;
        private bool _updateInfoBase;
        private List<Model.InfoBase> _infoBases = new List<Model.InfoBase>();

        internal List<Model.InfoBase> InfoBases { get => _infoBases; private set => _infoBases = value; }
        internal Model.InfoBase InfoBaseUpdate { get; set; }
        internal bool UpdateOnlySeansInfo { get; set; }

        internal ConnectToAgent(string serverName)
        {
            _serverName = serverName;
        }

        internal ConnectToAgent(UpdateStateEvents updateStateEvent, UpdateSessionsInfoEvents updateSessionsInfoEvents, string serverName)
        {
            _updateStateEvents = updateStateEvent;
            _updateSessionsInfoEvents = updateSessionsInfoEvents;
            _serverName = serverName;
        }

        internal async Task GetListBaseAsync()
        {
            await Task.Run(() => GetListBaseFromComAsync());
        }

        internal void SetListInfoBases(List<Model.InfoBase> listInfoBases)
        {
            InfoBases = listInfoBases;
            _updateInfoBase = InfoBaseUpdate != null;
        }

        internal async Task GetListBaseFromComAsync(bool removeBasesWhereNoAccess = true)
        {
            InitializeComConnector();

            ListNoAccessBase.List.Clear();

            if (UpdateOnlySeansInfo)
                for (int i = 0; i < InfoBases.Count; i++)
                    InfoBases[i].ClearSessionInfo();

            if (_updateInfoBase
                && InfoBaseUpdate != null)
            {
                InfoBaseUpdate.ConnectionCount = 0;
                InfoBaseUpdate.ClearSessionInfo();
            }

            await FillInfoBasesAllClusters();

            if (removeBasesWhereNoAccess)
                InfoBases.RemoveAll(f => !f.HaveAccess);

            InfoBases.Sort((a, b) => (b.NameToUpper.CompareTo(a.NameToUpper)));
        }

        internal void TerminateSessions(List<Model.Session> sessions)
        {
            InitializeComConnector();

            if (sessions.Count == 0)
                return;

            try
            {
                foreach (IClusterInfo itemClusterInfo in _serverAgent.GetClusters())
                {
                    _serverAgent.Authenticate(itemClusterInfo, "", "");

                    for (int i = 0; i < sessions.Count; i++)
                    {
                        Model.Session elemetSession = sessions[i];

                        #region Disconnect
                        // Disconnect
                        //bool disconnected = false;
                        //foreach (IWorkingProcessInfo itemWorkProcess in _serverAgent.GetWorkingProcesses(itemClusterInfo))
                        //{
                        //    IWorkingProcessConnection workingProcessConnection = GetWorkingProcessConnection(itemWorkProcess);
                        //    foreach (IInfoBaseInfo itemInfoBaseInfo in workingProcessConnection.GetInfoBases())
                        //    {
                        //        if (itemInfoBaseInfo.Name.ToUpper() == elemetSession.InfoBaseShort.Name.ToUpper())
                        //        {
                        //            foreach (IInfoBaseConnectionInfo itemInfoBaseConnection in workingProcessConnection.GetInfoBaseConnections(itemInfoBaseInfo))
                        //            {
                        //                if (itemInfoBaseConnection.ConnID == elemetSession.ConnID)
                        //                {
                        //                    //workingProcessConnection.Disconnect(itemInfoBaseConnection);
                        //                    //disconnected = true;
                        //                    //break;
                        //                }
                        //            }
                        //        }
                        //        if (disconnected)
                        //            break;
                        //    }
                        //    if (disconnected)
                        //        break;
                        //} 
                        #endregion

                        // Terminate
                        foreach (ISessionInfo itemSessionInfo in _serverAgent.GetSessions(itemClusterInfo))
                        {
                            if (itemSessionInfo.infoBase.Name == elemetSession.InfoBaseShort.Name
                                && itemSessionInfo.SessionID == elemetSession.SessionID)
                            {
                                _serverAgent.TerminateSession(itemClusterInfo, itemSessionInfo);
                                break;
                            }
                        }

                        sessions.RemoveAt(i);
                    }

                    if (sessions.Count == 0)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new TerminateSessionException(ex.Message);
            }

        }

        #region Get info

        private async Task FillInfoBasesAllClusters()
        {
            Array clusters = _serverAgent.GetClusters();

            if (!UpdateOnlySeansInfo)
                _updateStateEvents.CountCluster += clusters.Length;

            foreach (IClusterInfo clusterInfo in clusters)
            {
                _serverAgent.Authenticate(clusterInfo, "", "");

                if (!UpdateOnlySeansInfo)
                    await FillInfoBasesAllWorkProcesses(clusterInfo);

                GetInfoSessions(clusterInfo);

                _updateSessionsInfoEvents.EvokeUpdateSessionsInfoEvent();

                _updateStateEvents.ICluster++;
            }
            _updateStateEvents.ClearState();
        }

        private async Task FillInfoBasesAllWorkProcesses(IClusterInfo clusterInfo)
        {
            Array workingProcesses = _serverAgent.GetWorkingProcesses(clusterInfo);

            _updateStateEvents.CountWorkProcesses += workingProcesses.Length;

            List<Task> tasks = new List<Task>();

            foreach (IWorkingProcessInfo workProcess in workingProcesses)
                tasks.Add(FillInfoBaseFromWorkProcessAsync(workProcess));

            await Task.WhenAll(tasks);

            foreach (Task<List<Model.InfoBase>> item in tasks)
            {
                foreach (Model.InfoBase itemResult in item.Result)
                {
                    if (itemResult != null)
                    {
                        Model.InfoBase infoBase = InfoBases.FirstOrDefault(f => f.NameToUpper == itemResult.NameToUpper);
                        if (infoBase == null)
                            InfoBases.Add(itemResult);
                        else
                        {
                            infoBase.ConnectionCount += itemResult.ConnectionCount;
                            infoBase.HaveAccess = itemResult.HaveAccess;
                        }
                    }
                }
            }
        }

        private async Task<List<Model.InfoBase>> FillInfoBaseFromWorkProcessAsync(IWorkingProcessInfo workProcess)
        {
            return await Task.Run(() =>
            {
                List<Model.InfoBase> list = FillInfoBaseFromWorkProcess(GetWorkingProcessConnection(workProcess));

                _updateStateEvents.IProcesses++;

                return list;
            });
        }

        private List<Model.InfoBase> FillInfoBaseFromWorkProcess(IWorkingProcessConnection workingProcessConnection)
        {
            List<Model.InfoBase> listInfoBasesTask = new List<Model.InfoBase>();

            Array infoBases = workingProcessConnection.GetInfoBases();

            _updateStateEvents.CountInfoBase += infoBases.Length;
            foreach (IInfoBaseInfo infoBaseInfo in infoBases)
            {
                if (!_updateInfoBase || infoBaseInfo.Name.ToUpper() == InfoBaseUpdate.NameToUpper)
                {
                    if (ListNoAccessBase.ListName.FirstOrDefault(f => f == infoBaseInfo.Name.ToUpper()) == null)
                    {
                        IInfoBaseConnectionInfo infoBaseConnectionComConsole = FillInfoBase(workingProcessConnection, infoBaseInfo, listInfoBasesTask);
                        if (infoBaseConnectionComConsole != null)
                            workingProcessConnection.Disconnect(infoBaseConnectionComConsole);
                    }
                }
                _updateStateEvents.IInfoBase++;
            }

            return listInfoBasesTask;
        }

        private IInfoBaseConnectionInfo FillInfoBase(IWorkingProcessConnection workingProcessConnection, IInfoBaseInfo infoBaseInfo, List<Model.InfoBase> listInfoBasesTask)
        {
            IInfoBaseConnectionInfo infoBaseConnectionComConsole = null;
            bool haveAccess = true;
            int connections = 0;
            try
            {
                foreach (IInfoBaseConnectionInfo infoBaseConnectionInfo in workingProcessConnection.GetInfoBaseConnections(infoBaseInfo))
                    if (infoBaseConnectionInfo.AppID == "COMConsole")
                        infoBaseConnectionComConsole = infoBaseConnectionInfo;
                    else if (infoBaseConnectionInfo.AppID != "SrvrConsole")
                        connections++;
            }
            catch (Exception)
            {
                if (ListNoAccessBase.ListName.FirstOrDefault(f => f == infoBaseInfo.Name.ToUpper()) == null)
                    ListNoAccessBase.ListName.Add(infoBaseInfo.Name.ToUpper());

                haveAccess = false;
            }

            Model.InfoBase infoBase = listInfoBasesTask.FirstOrDefault(f => f.NameToUpper == infoBaseInfo.Name.ToUpper());
            if (infoBase == null)
            {
                infoBase = new Model.InfoBase()
                {
                    Name = infoBaseInfo.Name,
                    Descr = infoBaseInfo.Descr,
                    HaveAccess = haveAccess
                };
                infoBase.ConnectionCount += connections;
                listInfoBasesTask.Add(infoBase);
            }
            else
            {
                infoBase.ConnectionCount += connections;
                infoBase.HaveAccess = haveAccess;
            }

            return infoBaseConnectionComConsole;
        }

        private void GetInfoSessions(IClusterInfo clusterInfo)
        {
            Array sessions = _serverAgent.GetSessions(clusterInfo);
            foreach (ISessionInfo sessionInfo in sessions)
            {
                if (_updateInfoBase && sessionInfo.infoBase.Name.ToUpper() != InfoBaseUpdate.NameToUpper)
                    continue;

                if (sessionInfo.AppID != "COMConsole"
                    && sessionInfo.AppID != "SrvrConsole")
                {
                    Model.InfoBase infoBase = InfoBases.FirstOrDefault(f => f.NameToUpper == sessionInfo.infoBase.Name.ToUpper());
                    if (infoBase != null)
                    {
                        infoBase.SessionCount++;
                        infoBase.DbProcTook += ((float)sessionInfo.dbProcTook / 1000);
                        infoBase.DbProcInfo += sessionInfo.dbProcInfo;
                        infoBase.ListSessions.Add(new Model.Session(clusterInfo, sessionInfo));
                    }
                }
            }
        }

        #endregion

        private IWorkingProcessConnection GetWorkingProcessConnection(IWorkingProcessInfo workProcess)
        {
            IWorkingProcessConnection workingProcessConnection;
            try
            {
                //workingProcessConnection = _comConnector.ConnectWorkingProcess($"{workProcess.HostName}:{workProcess.MainPort}");
                workingProcessConnection = _comConnector.ConnectWorkingProcess($"{_serverName}:{workProcess.MainPort}");
            }
            catch (Exception)
            {
                //throw new WorkingProcessException(ex.Message);
                return null;
            }

            AddAuthentificationWorkingProcess(workingProcessConnection);

            return workingProcessConnection;
        }

        private void InitializeComConnector()
        {
            if (string.IsNullOrWhiteSpace(_serverName))
            {
                throw new ArgumentException("Не указано имя сервера.");
            }

            try
            {
                _comConnector = new COMConnector();
            }
            catch (Exception ex)
            {
                throw new CreateV83ComConnector(ex.Message);
            }

            try
            {
                _serverAgent = _comConnector.ConnectAgent(_serverName);
            }
            catch (Exception ex)
            {
                throw new ConnectAgentException(ex.Message);
            }
        }

        private static void AddAuthentificationWorkingProcess(IWorkingProcessConnection workingProcessConnection)
        {
            workingProcessConnection.AddAuthentication("", "");
        }

        public void Dispose()
        {
            _serverAgent = null;
            _comConnector = null;
            ListNoAccessBase.List.Clear();
        }
    }
}
