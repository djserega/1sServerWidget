using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget
{
    internal delegate void UpdateStateEvent();

    internal class UpdateStateEvents : EventArgs
    {
        string _fillStateCluster = string.Empty;
        string _fillStateWorkProcesses = string.Empty;
        string _fillStateInfoBase = string.Empty;

        private double _countCluster;
        private double _countWorkProcesses;
        private double _countInfoBases;

        private double _iCluster;
        private double _iWorkProcesses;
        private double _iInfoBases;

        internal double CountCluster { get => _countCluster; set { _countCluster = EvokeStateMethodChangeProperties(value); } }
        internal double CountWorkProcesses { get => _countWorkProcesses; set { _countWorkProcesses = EvokeStateMethodChangeProperties(value); } }
        internal double CountInfoBase { get => _countInfoBases; set { _countInfoBases = EvokeStateMethodChangeProperties(value); } }

        internal double ICluster { get => _iCluster; set { _iCluster = EvokeStateMethodChangeProperties(value); } }
        internal double IProcesses { get => _iWorkProcesses; set { _iWorkProcesses = EvokeStateMethodChangeProperties(value); } }
        internal double IInfoBase { get => _iInfoBases; set { _iInfoBases = EvokeStateMethodChangeProperties(value); } }

        private double EvokeStateMethodChangeProperties(double value)
        {
            EvokeUpdateStateEvent();

            return value;
        }

        internal StateTypes TypeState { get; set; }


        internal event UpdateStateEvent UpdateStateEvent;

        internal void EvokeUpdateStateEvent()
        {
            if (UpdateStateEvent == null)
                return;

            UpdateStateEvent();
        }

        internal void ClearState()
        {
            _countCluster = 0;
            _countWorkProcesses = 0;
            _countInfoBases = 0;

            _iCluster = 0;
            _iWorkProcesses = 0;
            _iInfoBases = 0;

            EvokeUpdateStateEvent();
        }

        internal string GetState()
        {
            StringBuilder stringBuilder = new StringBuilder();

            switch (TypeState)
            {
                case StateTypes.Persent:
                    _fillStateCluster = _countCluster > 0 ? $"Кластеры {(int)((_iCluster / _countCluster) * 100)}%" : string.Empty;
                    _fillStateWorkProcesses = _countWorkProcesses > 0 ? $"Рабочие процессы {(int)((_iWorkProcesses / _countWorkProcesses) * 100)}%" : string.Empty;
                    _fillStateInfoBase = _countInfoBases > 0 ? $"Базы данных {(int)((_iInfoBases / _countWorkProcesses) / (_countInfoBases / _countWorkProcesses) * 100)}%" : string.Empty;
                    break;
                default:
                    _fillStateCluster = _countCluster > 0 ? $"Кластеры {_iCluster}/{_countCluster}" : string.Empty;
                    _fillStateWorkProcesses = _countWorkProcesses > 0 ? $"Рабочие процессы {_iWorkProcesses}/{_countWorkProcesses}" : string.Empty;
                    _fillStateInfoBase = _countInfoBases > 0 ? $"Базы данных {(_iInfoBases / _countWorkProcesses)}/{(_countInfoBases / _countWorkProcesses)}" : string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(_fillStateCluster))
            {
                stringBuilder.Append(_fillStateCluster);
                stringBuilder.Append("; ");
            }
            if (!string.IsNullOrEmpty(_fillStateWorkProcesses))
            {
                stringBuilder.Append(_fillStateWorkProcesses);
                stringBuilder.Append("; ");
            }
            if (!string.IsNullOrEmpty(_fillStateInfoBase))
            {
                stringBuilder.Append(_fillStateInfoBase);
                stringBuilder.Append("; ");
            }

            return stringBuilder.ToString();
        }
        internal int GetStateStatusBar()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (TypeState != StateTypes.StatusBar)
                return 0;

            double currentState = _iCluster + _iWorkProcesses + _iInfoBases;
            double allState = _countCluster + _countWorkProcesses + _countInfoBases;

            int progress = (int)(currentState * 100 / allState);

            return progress;
        }
    }


    internal delegate void UpdateSessionsInfoEvent();

    internal class UpdateSessionsInfoEvents : EventArgs
    {
        internal Model.InfoBase InfoBase { get; set; }

        internal event UpdateSessionsInfoEvent UpdateSessionsInfoEvent;

        internal void EvokeUpdateSessionsInfoEvent()
        {
            if (UpdateSessionsInfoEvent == null)
                return;

            UpdateSessionsInfoEvent();
        }
    }
}
