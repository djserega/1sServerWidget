using V83;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget.Model
{
    public class Session
    {
        public IClusterInfo ClusterInfo { get;}
        public IInfoBaseShort InfoBaseShort { get; }
        public ISessionInfo SessionInfo { get; }
        public string AppID { get; }
        public int SessionID { get; }
        public string UserName { get; }
        public IWorkingProcessInfo Process { get;  }
        public IConnectionShort Connection { get; }
        public float DbProcTook { get; }
        public string DbProcInfo { get; }
        public DateTime StartedAt { get; }
        public int ConnID { get; }

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
    }
}
