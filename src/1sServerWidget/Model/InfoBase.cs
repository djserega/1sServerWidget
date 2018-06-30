using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _1sServerWidget.Model
{
    public class InfoBase : INotifyPropertyChanged
    {

        #region Constructor

        public InfoBase()
        {
            ListSessions = new List<Session>();
        }

        #endregion

        #region Fields

        private string _name;
        private string _nameToUpper;
        private string _descr;
        private int _connectionCount;
        private int _sessionCount;
        private bool _haveAccess;
        private string _dbProcInfo;
        private float _dbProcTook;
        private string _lastUpdate;

        #endregion

        #region Properties

        public string Name { get => _name; set { _name = value; _nameToUpper = value.ToUpper(); SetLastUpdate(); NotifyPropertyChanged(); } }
        public string NameToUpper { get => _nameToUpper; }
        public string Descr { get => _descr; set { _descr = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public int ConnectionCount { get => _connectionCount; set { _connectionCount = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public int SessionCount { get => _sessionCount; set { _sessionCount = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public string DbProcInfo { get => _dbProcInfo; set { _dbProcInfo = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public float DbProcTook { get => _dbProcTook; set { _dbProcTook = value; SetLastUpdate(); NotifyPropertyChanged(); } }

        public bool HaveAccess { get => _haveAccess; set { _haveAccess = value; SetLastUpdate(); NotifyPropertyChanged(); } }
        public string LastUpdate { get => _lastUpdate; }

        internal List<Session> ListSessions { get; set; }

        #endregion

        #region Event changed

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Internal methods

        internal void Fill(InfoBase infoBase)
        {
            _name = infoBase.Name;
            _nameToUpper = infoBase.NameToUpper;
            _descr = infoBase.Descr;
            _connectionCount = infoBase.ConnectionCount;
            _sessionCount = infoBase.SessionCount;
            _haveAccess = infoBase.HaveAccess;
            _dbProcInfo = infoBase.DbProcInfo;
            _dbProcTook = infoBase.DbProcTook;
            SetLastUpdate();
        }

        internal void ClearSessionInfo()
        {
            _sessionCount = 0;
            _dbProcInfo = string.Empty;
            _dbProcTook = 0;
            ListSessions.Clear();
        }

        #endregion

        #region Public methods

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Private methods

        private void SetLastUpdate()
        {
            _lastUpdate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        #endregion

    }
}
