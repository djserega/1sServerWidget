using V83;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _1sServerWidget.Model
{
    public class Session : INotifyPropertyChanged
    {
        public IClusterInfo ClusterInfo { get; private set; }
        public IInfoBaseShort InfoBaseShort { get; private set; }
        public ISessionInfo SessionInfo { get; private set; }
        public string AppID { get; private set; }
        public int SessionID { get; private set; }
        public string UserName { get; private set; }
        public IWorkingProcessInfo Process { get; private set; }
        public IConnectionShort Connection { get; private set; }
        public float DbProcTook { get; private set; }
        public string DbProcInfo { get; private set; }
        public DateTime StartedAt { get; private set; }
        public int ConnID { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Session(IClusterInfo clusterInfo, ISessionInfo sessionInfo)
        {
            ClusterInfo = clusterInfo;
            InfoBaseShort = sessionInfo.infoBase;
            SessionInfo = sessionInfo;
            AppID = sessionInfo.AppID;
            SessionID = sessionInfo.SessionID;
            UserName = sessionInfo.userName;
            Process = sessionInfo.process;
            Connection = sessionInfo.connection;
            DbProcInfo = sessionInfo.dbProcInfo;
            DbProcTook = ((float)sessionInfo.dbProcTook / 1000);
            StartedAt = sessionInfo.StartedAt;
            ConnID = Connection == null ? 0 : Connection.ConnID;
        }

        internal void Fill(Session session)
        {
            ClusterInfo = session.ClusterInfo;
            InfoBaseShort = session.InfoBaseShort;
            SessionInfo = session.SessionInfo;
            AppID = session.AppID;
            SessionID = session.SessionID;
            UserName = session.UserName;
            Process = session.Process;
            Connection = session.Connection;
            DbProcTook = session.DbProcTook;
            DbProcInfo = session.DbProcInfo;
            StartedAt = session.StartedAt;
            ConnID = session.ConnID;
        }
    }
}
