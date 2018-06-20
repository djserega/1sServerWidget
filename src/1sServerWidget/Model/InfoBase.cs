using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget.Model
{
    public class InfoBase : INotifyPropertyChanged
    {
        private string _name;
        private string _nameToUpper;
        private string _descr;
        private int _connectionCount;
        private int _sessionCount;
        private bool _haveAccess;
        private string _dbProcInfo;
        private float _dbProcTook;

        public string Name { get => _name; set { _name = value; _nameToUpper = value.ToUpper(); SetLastUpdate(); NotifyPropertyChanged(); } }
        public string NameToUpper { get => _nameToUpper; }
        public string Descr { get => _descr; set { _descr = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public int ConnectionCount { get => _connectionCount; set { _connectionCount = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public int SessionCount { get => _sessionCount; set { _sessionCount = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public string DbProcInfo { get => _dbProcInfo; set { _dbProcInfo = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public float DbProcTook { get => _dbProcTook; set { _dbProcTook = value; SetLastUpdate(); NotifyPropertyChanged(); } }

        public bool HaveAccess { get => _haveAccess; set { _haveAccess = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public string LastUpdate { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal List<Session> ListSessions { get; set; }

        public InfoBase()
        {
            ListSessions = new List<Session>();
        }

        private void SetLastUpdate()
        {
            LastUpdate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        internal void ClearSessionInfo()
        {
            _sessionCount = 0;
            _dbProcInfo = string.Empty;
            _dbProcTook = 0;
            ListSessions.Clear();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
