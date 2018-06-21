﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using timers = System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _1sServerWidget
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UpdateStateEvents _updateStateEvents;
        private UpdateSessionsInfoEvents _updateSessionsInfoEvents;

        private int _minUpdateSession;
        private timers.Timer _timer = new timers.Timer();
        private bool _updateListIsRunning;
        private readonly ObservableCollection<Model.InfoBase> _listBases = new ObservableCollection<Model.InfoBase>();

        public ObservableCollection<Model.InfoBase> ListBases { get => _listBases; }
        public string ServerName { get; set; }
        public string TextState { get; private set; }
        public string LastUpdate { get; private set; }
        public int MinUpdateSession { get => _minUpdateSession; set { _minUpdateSession = value; StartStopUpdateSession(); } }
        public int ValueProgressBar { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DefaultValue defaultValue = new DefaultValue();
            ServerName = defaultValue.ServerName;
            _minUpdateSession = defaultValue.MinUpdateSession;

            DataContext = this;

            _updateStateEvents = new UpdateStateEvents() { TypeState = StateTypes.StatusBar };
            _updateStateEvents.UpdateStateEvent += UpdateStateEvents_UpdateStateEvent;

            _updateSessionsInfoEvents = new UpdateSessionsInfoEvents();

            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new ThreadStart(delegate { GetListBases(updateOnlySeansInfo: true); }));
        }

        private void UpdateStateEvents_UpdateStateEvent()
        {
            int ValueProgressBarNew = _updateStateEvents.GetStateStatusBar();

            if (ValueProgressBar != ValueProgressBarNew)
            {
                ValueProgressBar = ValueProgressBarNew;

                Dispatcher.Invoke(new ThreadStart(delegate
                {
                    BindingOperations.GetBindingExpression(ProgressBar, ProgressBar.ValueProperty).UpdateTarget();
                }));
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FormMainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            new DefaultValue() { ServerName = ServerName }.SetValueByKey("ServerName");
            GetListBases();
        }

        private async void GetListBases(Model.InfoBase infoBaseUpdate = null, bool updateOnlySeansInfo = false)
        {
            if (_updateListIsRunning)
                return;

            _updateListIsRunning = true;

            if (!updateOnlySeansInfo)
                ButtonConnect.Content = "Обновление";

            ButtonConnect.IsEnabled = false;

            try
            {
                ConnectToAgent connectToAgent = new ConnectToAgent(_updateStateEvents, _updateSessionsInfoEvents, ServerName);

                if (infoBaseUpdate != null)
                {
                    connectToAgent.InfoBaseUpdate = infoBaseUpdate;
                    connectToAgent.SetListInfoBases(_listBases.ToList());
                }
                else if (updateOnlySeansInfo)
                    connectToAgent.SetListInfoBases(_listBases.ToList());

                connectToAgent.UpdateOnlySeansInfo = updateOnlySeansInfo;

                await connectToAgent.GetListBaseAsync();

                RefreshDataContextListBase(connectToAgent.InfoBases);

                LastUpdate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                BindingOperations.GetBindingExpression(TextBlockLastUpdate, TextBlock.TextProperty).UpdateTarget();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                Focus();
                TextBoxServerName.Focus();
            }
            catch (CreateV83ComConnector ex)
            {
                MessageBox.Show($"Не удалось создать COMConnector.\n{ex.Message}");
            }
            catch (ConnectAgentException ex)
            {
                MessageBox.Show($"Ошибка соединения с сервером.\n{ex.Message}");
            }
            catch (WorkingProcessException ex)
            {
                MessageBox.Show($"Ошибка соединения с рабочим процессом.\n{ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ButtonConnect.IsEnabled = true;

            if (!updateOnlySeansInfo)
                ButtonConnect.Content = "Подключиться";

            StartStopUpdateSession();

            _updateListIsRunning = false;
        }

        private void RefreshDataContextListBase(List<Model.InfoBase> newListBases)
        {      
            List<Model.InfoBase> deletingRow = new List<Model.InfoBase>();
            foreach (Model.InfoBase itemRow in _listBases)
            {
                Model.InfoBase newInfoBase = newListBases.FirstOrDefault(f => f.NameToUpper == itemRow.NameToUpper);
                if (newInfoBase == null)
                {
                    deletingRow.Add(itemRow);
                }
                else
                {
                    itemRow.Fill(newInfoBase);
                    newListBases.Remove(newInfoBase);
                }
            }
            foreach (Model.InfoBase item in deletingRow)
            {
                _listBases.Remove(item);
            }
            foreach (Model.InfoBase item in newListBases)
            {
                _listBases.Add(item);
            }
        }

        private void MenuItemUpdateInfo_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridListBase.SelectedItem is Model.InfoBase infoBase)
            {
                if (infoBase != null)
                    GetListBases(infoBase);
            }
        }

        private void MenuItemUpdateDbProcTook_Click(object sender, RoutedEventArgs e)
        {
            GetListBases(updateOnlySeansInfo: true);
        }

        private void StartStopUpdateSession()
        {
            if (_minUpdateSession == 0 || _listBases.Count == 0)
            {
                _timer.Stop();
                BorderMinUpdateSession.Background = new SolidColorBrush();
            }
            else
            {
                _timer.Interval = _minUpdateSession * 1000;
                _timer.Start();
                BorderMinUpdateSession.Background = new SolidColorBrush(Colors.Gold);
            }
        }

        private void TextBoxMinUpdateSession_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartStopUpdateSession();

            BindingOperations.GetBindingExpression(TextBoxMinUpdateSession, TextBox.TextProperty).UpdateSource();

            new DefaultValue() { MinUpdateSession = _minUpdateSession}.SetValueByKey("MinUpdateSession");
        }

        private void DataGridListBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridListBase.SelectedItem is Model.InfoBase infoBase)
            {
                if (infoBase.ListSessions.Count > 0)
                {
                    FormSessionsInfoBase form = new FormSessionsInfoBase(infoBase) { Owner = this, UpdateSessionsInfoEvents = _updateSessionsInfoEvents };
                    form.ShowDialog();
                }
            }
        }
    }
}
