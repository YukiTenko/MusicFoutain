using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MusicFountain.UI.Hardware.ListHardware
{
    /// <summary>
    /// Interaction logic for UI_ListHardware.xaml
    /// </summary>

    public partial class UI_ListHardware : Page
    {
        // Support class and constants
        public class DataGrid_HardwareConfig
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string serverURL { get; set; }
            public string connectionState { get; set; }

        }
        public class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_HardwareConfig> listHardwareConfig;
        }
        
        private List<string> LIST_HARDWARE_TYPE = new List<string>() {"Pump A/D & Power - PLC", "Valve On/Off & Stepper - PLC", "Valve Stepper & LED 485 - PLC", "LED DMX - Artnet"};

        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver = new DataGrid_AllData_Observer();
            // For backend data
        private List<Data.Data_Config_Hardware> listHardwareConfig;

        // Event
        public event EventHandler<ObservableCollection<DataGrid_HardwareConfig>> updateNewListHardwareEvent;
        public event EventHandler<int> reconnectHardwareEvent;

        public UI_ListHardware()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listHardwareConfig = new ObservableCollection<DataGrid_HardwareConfig>();
                }
                {// For backend process
                    listHardwareConfig = new List<Data.Data_Config_Hardware>();
                }
            }

            {// First time UI Update
                {// Set source for data grid first
                    hardwareType_ComboBox.ItemsSource = LIST_HARDWARE_TYPE;
                    listHardwareData_DataGrid.ItemsSource = dataGridAllDataObserver.listHardwareConfig;

                    CollectionViewSource.GetDefaultView(listHardwareData_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                }
            }
        }

        // Public methods
        public void Update_ListHardware(List<Data.Data_Config_Hardware> newList)
        {
            listHardwareConfig = newList;
            dataGridAllDataObserver.listHardwareConfig.Clear();
            for (int i = 0; i < listHardwareConfig.Count; i++)
            {
                DataGrid_HardwareConfig hardwareConfig_DataGridBuf = new DataGrid_HardwareConfig();
                hardwareConfig_DataGridBuf.id = listHardwareConfig[i].hardwareID;
                hardwareConfig_DataGridBuf.name = listHardwareConfig[i].name;
                hardwareConfig_DataGridBuf.type = listHardwareConfig[i].type;
                switch (hardwareConfig_DataGridBuf.type)
                {
                    case "Valve Stepper - PLC":
                    case "Valve On/Off - PLC":
                    case "LED 485 - PLC":
                    case "Pump A/D & Power - PLC":
                    case "Valve On/Off & Stepper - PLC":
                    case "Valve Stepper & LED 485 - PLC":
                        {
                            hardwareConfig_DataGridBuf.serverURL = listHardwareConfig[i].modbusServerURL;
                        }
                        break;
                    case "LED DMX - Artnet":
                        {
                            hardwareConfig_DataGridBuf.serverURL = listHardwareConfig[i].artnetServerURL;
                        }
                        break;
                    default:
                        {
                            hardwareConfig_DataGridBuf.serverURL = "";
                        }
                        break;

                }
                hardwareConfig_DataGridBuf.connectionState = listHardwareConfig[i].serverConnectionState;
                dataGridAllDataObserver.listHardwareConfig.Add(hardwareConfig_DataGridBuf);
            }

            CollectionViewSource.GetDefaultView(listHardwareData_DataGrid.ItemsSource).Refresh();
        }
        public void Update_HardwareConnectionState(int hardwareIDBuf, string connectionconnectionStateBuf)
        {
            for (int i = 0; i < dataGridAllDataObserver.listHardwareConfig.Count; i++)
            {
                if (hardwareIDBuf == dataGridAllDataObserver.listHardwareConfig[i].id)
                {
                    dataGridAllDataObserver.listHardwareConfig[i].connectionState = connectionconnectionStateBuf;
                }
            }
            CollectionViewSource.GetDefaultView(listHardwareData_DataGrid.ItemsSource).Refresh();
        }
        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(listHardwareData_DataGrid.ItemsSource).Refresh();
        }


        // Private methods
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (updateNewListHardwareEvent != null)
            {
                updateNewListHardwareEvent.Invoke(this, dataGridAllDataObserver.listHardwareConfig);
            }
            
        }
        private void Button_Reconnect_OnClick(object sender, RoutedEventArgs e)
        {
            int index = listHardwareData_DataGrid.SelectedIndex;
            if (dataGridAllDataObserver.listHardwareConfig[index].connectionState == "Connected")
            {
                MessageBox.Show("Thiết bị đã được kết nối");
            }
            else
            {
                reconnectHardwareEvent.Invoke(this, dataGridAllDataObserver.listHardwareConfig[index].id);
            }
        }

    }
}
