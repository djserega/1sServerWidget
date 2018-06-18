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
        private int _countCluster;
        private int _countWorkProcesses;
        private int _countInfoBases;

        private int _iCluster;
        private int _iWorkProcesses;
        private int _iInfoBases;

        internal int CountCluster { get => _countCluster; set { _countCluster = value; EvokeUpdateStateEvent(); } }
        internal int CountWorkProcesses { get => _countWorkProcesses; set { _countWorkProcesses = value; EvokeUpdateStateEvent(); } }
        internal int CountInfoBase { get => _countInfoBases; set { _countInfoBases = value; EvokeUpdateStateEvent(); } }

        internal int ICluster { get => _iCluster; set { _iCluster = value; EvokeUpdateStateEvent(); } }
        internal int IProcesses { get => _iWorkProcesses; set { _iWorkProcesses = value; EvokeUpdateStateEvent(); } }
        internal int IInfoBase { get => _iInfoBases; set { _iInfoBases = value; EvokeUpdateStateEvent(); } }


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

            string _fillStateCluster = _countCluster > 0 ? $"Кластеры {_iCluster}/{_countCluster}" : string.Empty;
            string _fillStateWorkProcesses = _countWorkProcesses > 0 ? $"Рабочие процессы {_iWorkProcesses}/{_countWorkProcesses}" : string.Empty;
            string _fillStateInfoBase = _countInfoBases> 0 ? $"Базы данных {(_iInfoBases / _countWorkProcesses)}/{(_countInfoBases / _countWorkProcesses)}" : string.Empty;

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
