using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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

namespace MusicFountain.UI.Log
{
    public partial class UI_Log : Page
    {
        // Support class
        private class DataGrid_LogInfo
        {
            public int id { get; set; }
            public int type { get; set; }
            public string time { get; set; }
            public string text { get; set; }
        }
        private class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_LogInfo> listLogs;
        }

        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
        // For backend data
        private List<string> listLogFiles;

        // Events
        public event EventHandler<string> chosenLogFileChanged;
        public event EventHandler<int> clearLogClick;

        public UI_Log()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listLogs = new ObservableCollection<DataGrid_LogInfo>();
                }
                {// For backend process
                    listLogFiles = new List<string>();
                }
            }

            {// First time UI Update
                {// Set source for data grid first
                    listLog_DataGrid.ItemsSource = dataGridAllDataObserver.listLogs;
                    CollectionViewSource.GetDefaultView(listLog_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                    RefreshListLogFiles();
                    logFileSelection_ComboBox.ItemsSource = listLogFiles;
                }
            }
        }

        // Private methods
        private void LogFileSelection_ComboBoxChanged_EventHandle(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (logFileSelection_ComboBox.SelectedItem.ToString() != "")
                {
                    chosenLogFileChanged.Invoke(this, logFileSelection_ComboBox.SelectedItem.ToString());
                }
            }
            catch (Exception)
            {

            }
        }

        private void Refresh_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            RefreshListLogFiles();
            RefreshListLogs();
        }

        private void ClearLog_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            clearLogClick.Invoke(this, 0);
        }


        // Public methods
        public void RefreshListLogFiles()
        {
            string[] listLogFilesBuf = Directory.GetFiles("../Data/Log");
            listLogFiles.Clear();
            foreach (string logFileBuf in listLogFilesBuf)
            {
                listLogFiles.Add(logFileBuf);
            }
        }
        public void RefreshListLogs()
        {
            try
            {
                if (logFileSelection_ComboBox.SelectedItem.ToString() != "")
                {
                    chosenLogFileChanged.Invoke(this, logFileSelection_ComboBox.SelectedItem.ToString());
                }
            }
            catch (Exception)
            {

            }
        }

        public void UpdateListLogs(List<Data.Data_Log_Info> newListLogs)
        {
            dataGridAllDataObserver.listLogs.Clear();
            for (int i = 0; i < newListLogs.Count; i++)
            {
                DataGrid_LogInfo newLogInfo = new DataGrid_LogInfo();
                newLogInfo.id = i + 1;
                newLogInfo.type = newListLogs[i].type;
                newLogInfo.time = newListLogs[i].time;
                newLogInfo.text = newListLogs[i].text;
                dataGridAllDataObserver.listLogs.Add(newLogInfo);
            }
            CollectionViewSource.GetDefaultView(listLog_DataGrid.ItemsSource).Refresh();
        }

        public void ApplicationExit()
        {
        }
    }
}
