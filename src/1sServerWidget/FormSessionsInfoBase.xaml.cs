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

        private readonly ObservableCollection<Model.Session> _sessions = new ObservableCollection<Model.Session>();
        private readonly string[] _listNotTerminatedAppIDSessions = new string[4] { "BackgroundJob", "Designer", "COMConsole", "SrvrConsole" };

        public ObservableCollection<Model.Session> Sessions { get => _sessions; }

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
                RefreshDataContextListBase(InfoBase.ListSessions.ToList());
            }));
        }

        private void RefreshDataContextListBase(List<Model.Session> newListBases)
        {
            List<Model.Session> deletingRow = new List<Model.Session>();
            foreach (Model.Session item in _sessions)
            {
                Model.Session newInfoBase = newListBases.FirstOrDefault(f => f.ConnID == item.ConnID);
                if (newInfoBase == null)
                {
                    deletingRow.Add(item);
                }
                else
                {
                    item.Fill(newInfoBase);
                    newListBases.Remove(newInfoBase);
                }
            }
            foreach (Model.Session item in deletingRow)
            {
                _sessions.Remove(item);
            }
            foreach (Model.Session item in newListBases)
            {
                _sessions.Add(item);
            }
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
                using (ConnectToAgent connector = new ConnectToAgent(((MainWindow)Owner).ServerName))
                {
                    try
                    {
                        connector.TerminateSessions(listTerminateSessions);
                    }
                    catch (TerminateSessionException ex)
                    {
                        MessageBox.Show($"Ошибка завершения сеанса\n{ex.Message}");
                    }
                }
            }
        }

        private void MenuItemTerminateSession_Click(object sender, RoutedEventArgs e)
        {
            TerminateSession();
        }
    }
}
