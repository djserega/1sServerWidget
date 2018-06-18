using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _1sServerWidget
{
    /// <summary>
    /// Логика взаимодействия для FormSessionsInfoBase.xaml
    /// </summary>
    public partial class FormSessionsInfoBase : Window
    {
        private UpdateSessionsInfoEvents _updateSessionsInfoEvents;
        internal UpdateSessionsInfoEvents UpdateSessionsInfoEvents
        {
            get => _updateSessionsInfoEvents;
            set
            {
                _updateSessionsInfoEvents = value;
                _updateSessionsInfoEvents.UpdateSessionsInfoEvent += _updateSessionsInfoEvents_UpdateSessionsInfoEvent;
            }
        }

        private ObservableCollection<Model.Session> Sessions = new ObservableCollection<Model.Session>();
        private readonly string[] _listNotTerminatedAppIDSessions = new string[4] { "BackgroundJob", "Designer", "COMConsole", "SrvrConsole" };

        private void _updateSessionsInfoEvents_UpdateSessionsInfoEvent()
        {
            SetItemSourceDataGridSessions();
        }

        public Model.InfoBase InfoBase { get; }

        public FormSessionsInfoBase(Model.InfoBase infoBase)
        {
            InitializeComponent();

            InfoBase = infoBase;

            SetItemSourceDataGridSessions();

            DataContext = this;
        }

        private void SetItemSourceDataGridSessions()
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                Sessions.Clear();
                foreach (Model.Session item in InfoBase.ListSessions)
                    Sessions.Add(item);
                DataGridSessions.ItemsSource = Sessions;
            }));
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowSessionInfoBase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void WindowSessionInfoBase_Loaded(object sender, RoutedEventArgs e)
        {
            Left = (Owner.Width - Width) / 2 + Owner.Left;
            Top = (Owner.Height - Height) / 2 + Owner.Top;
        }

        private void ButtonTerminateSession_Click(object sender, RoutedEventArgs e)
        {
            TerminateSession();
        }

        private void TerminateSession()
        {
            List<Model.Session> listTerminateSessions = new List<Model.Session>();

            if (DataGridSessions.SelectedItem is Model.Session session)
                listTerminateSessions.Add(session);

            //foreach (Model.Session item in Sessions)
            //{
            //    if (string.IsNullOrEmpty(_listNotTerminatedAppIDSessions.FirstOrDefault(f => f == item.AppID)))
            //        listTerminateSessions.Add(item);
            //}

            if (listTerminateSessions.Count > 0)
            {
                ConnectToAgent connector = new ConnectToAgent(((MainWindow)Owner).ServerName);
                connector.TerminateSessions(listTerminateSessions);
            }
        }

        private void MenuItemTerminateSession_Click(object sender, RoutedEventArgs e)
        {
            TerminateSession();
        }
    }
}
