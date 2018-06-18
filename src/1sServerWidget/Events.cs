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
        private string _fillStateCluster;
        private string _fillStateSession;
        private string _fillStateWorkProcesses;
        private string _fillStateInfoBase;

        internal string FillStateCluster { get => _fillStateCluster; set { _fillStateCluster = value; EvokeUpdateStateEvent(); } }
        internal string FillStateSession { get => _fillStateSession; set { _fillStateSession = value; EvokeUpdateStateEvent(); } }
        internal string FillStateWorkProcesses { get => _fillStateWorkProcesses; set { _fillStateWorkProcesses = value; EvokeUpdateStateEvent(); } }
        internal string FillStateInfoBase { get => _fillStateInfoBase; set { _fillStateInfoBase = value; EvokeUpdateStateEvent(); } }


        internal event UpdateStateEvent UpdateStateEvent;

        internal void EvokeUpdateStateEvent()
        {
            if (UpdateStateEvent == null)
                return;

            UpdateStateEvent();
        }

        internal void ClearState()
        {
            _fillStateCluster = string.Empty;
            _fillStateSession = string.Empty;
            _fillStateWorkProcesses = string.Empty;
            _fillStateInfoBase = string.Empty;

            EvokeUpdateStateEvent();
        }

        internal string GetState()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(_fillStateCluster))
            {
                stringBuilder.Append(_fillStateCluster);
                stringBuilder.Append("; ");
            }
            if (!string.IsNullOrEmpty(_fillStateSession))
            {
                stringBuilder.Append(_fillStateSession);
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
