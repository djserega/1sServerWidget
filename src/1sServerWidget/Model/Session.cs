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
        public IClusterInfo ClusterInfo { get; set; }
        public IInfoBaseShort InfoBaseShort { get; set; }
        public ISessionInfo SessionInfo { get; set; }
        public string AppID { get; set; }
        public int SessionID { get; set; }
        public string UserName { get; set; }
        public dynamic Process { get; set; }
        public dynamic Connection { get; set; }
        public float DbProcTook { get; set; }
        public string DbProcInfo { get; set; }

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
        }
    }
}
