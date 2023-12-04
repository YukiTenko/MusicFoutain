using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace MusicFountain.UI.Hardware.Group
{
    /// <summary>
    /// Interaction logic for UI_GroupAnalog.xaml
    /// </summary>

    public partial class UI_Group_PumpADAndPower_PLC : Page
    {
        // Support class and constants
        public class DataGrid_GroupConfig
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string effect { get; set; }
            public float freqMax { get; set; }
            public float timeGoOn { get; set; }
            public float timeHoldOn { get; set; }
            public float freqMin { get; set; }
            public float timeHoldOff { get; set; }
            public float timeGoOff { get; set; }
        }

        public class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_GroupConfig> listGroup { get; set; }
        }
        private List<string> LIST_GROUP_TYPE = new List<string>() { "Pump Digital - PLC", "Pump Analog - PLC", "Pump Analog Dual - PLC", "System Power" };

        // Variables
            // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
            // For backend data
        private Data.Data_Config_Hardware hardwareConfig;

            // Events
        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;
        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        public UI_Group_PumpADAndPower_PLC()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listGroup = new ObservableCollection<DataGrid_GroupConfig>();
                }
                {// For backend process
                    hardwareConfig = new Data.Data_Config_Hardware();
                }
            }

            {// First time UI Update
                {// Set source for data grid first
                    groupType_ComboBox.ItemsSource = LIST_GROUP_TYPE;

                    listGroup_DataGrid.ItemsSource = dataGridAllDataObserver.listGroup;
                    CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                }
            }
        }
        // Public methods
        public void Update_HardwareConfig(Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            dataGridAllDataObserver.listGroup.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                DataGrid_GroupConfig dataGridBuffer = new DataGrid_GroupConfig();
                dataGridBuffer.id = hardwareConfig.listGroup[i].groupID;
                dataGridBuffer.name = hardwareConfig.listGroup[i].name;
                dataGridBuffer.type = hardwareConfig.listGroup[i].type;
                dataGridBuffer.effect = hardwareConfig.listGroup[i].effectSave;
                switch (dataGridBuffer.type)
                {
                    case "Pump Digital - PLC":
                        {
                            dataGridBuffer.startDev = "Bơm " + hardwareConfig.listGroup[i].startDevID.ToString();
                            dataGridBuffer.endDev = "Bơm " + hardwareConfig.listGroup[i].endDevID.ToString();
                        }
                        break;
                    case "Pump Analog - PLC":
                    case "Pump Analog Dual - PLC":
                        {
                            dataGridBuffer.startDev = "Biến tần " + hardwareConfig.listGroup[i].startDevID.ToString();
                            dataGridBuffer.endDev = "Biến tần " + hardwareConfig.listGroup[i].endDevID.ToString();
                            dataGridBuffer.freqMax = hardwareConfig.listGroup[i].freqMax;
                            dataGridBuffer.timeGoOn = hardwareConfig.listGroup[i].timeGoOn;
                            dataGridBuffer.timeHoldOn = hardwareConfig.listGroup[i].timeHoldOn;
                            dataGridBuffer.freqMin = hardwareConfig.listGroup[i].freqMin;
                            dataGridBuffer.timeGoOff = hardwareConfig.listGroup[i].timeGoOff;
                            dataGridBuffer.timeHoldOff = hardwareConfig.listGroup[i].timeHoldOff;
                        }
                        break;
                    case "System Power":
                    default:
                        {

                        }
                        break;
                }
                dataGridAllDataObserver.listGroup.Add(dataGridBuffer);
            }
        }

        // Private methods
        private void GroupSave_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            if (updateClickEvent != null)
            {
                Data.Data_Config_Hardware newHardwareGroupInfo = new Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listInverter = hardwareConfig.listInverter;
                newHardwareGroupInfo.optionInverter = hardwareConfig.optionInverter;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < dataGridAllDataObserver.listGroup.Count; i++)
                {
                    Data.Data_Config_Hardware_Group newGroupBuf = new Data.Data_Config_Hardware_Group();
                    newGroupBuf.hardwareID = newHardwareGroupInfo.hardwareID;
                    newGroupBuf.groupID = dataGridAllDataObserver.listGroup[i].id;
                    newGroupBuf.name = dataGridAllDataObserver.listGroup[i].name;
                    newGroupBuf.type = dataGridAllDataObserver.listGroup[i].type;
                    newGroupBuf.effectSave = dataGridAllDataObserver.listGroup[i].effect;
                    try
                    {
                        switch (newGroupBuf.type)
                        {
                            case "Pump Digital - PLC":
                                {
                                    newGroupBuf.startDevID = int.Parse(dataGridAllDataObserver.listGroup[i].startDev.Replace("Bơm ", ""));
                                    newGroupBuf.endDevID = int.Parse(dataGridAllDataObserver.listGroup[i].endDev.Replace("Bơm ", ""));
                                }
                                break;
                            case "Pump Analog - PLC":
                            case "Pump Analog Dual - PLC":
                                {
                                    newGroupBuf.startDevID = int.Parse(dataGridAllDataObserver.listGroup[i].startDev.Replace("Biến tần ", ""));
                                    newGroupBuf.endDevID = int.Parse(dataGridAllDataObserver.listGroup[i].endDev.Replace("Biến tần ", ""));
                                    newGroupBuf.freqMax = dataGridAllDataObserver.listGroup[i].freqMax;
                                    newGroupBuf.timeGoOn = dataGridAllDataObserver.listGroup[i].timeGoOn;
                                    newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                                    newGroupBuf.freqMin = dataGridAllDataObserver.listGroup[i].freqMin;
                                    newGroupBuf.timeGoOff = dataGridAllDataObserver.listGroup[i].timeGoOff;
                                    newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                                    newGroupBuf.cycle = dataGridAllDataObserver.listGroup[i].timeGoOn;
                                }
                                break;
                            case "System Power":
                            default:
                                {

                                }
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Vui lòng chọn danh sách thiết bị trong nhóm trước khi lưu . Hiên tại sẽ lưu danh sách thiết bị mặc định .");
                        newGroupBuf.startDevID = 1;
                        newGroupBuf.endDevID = 1;
                        newGroupBuf.freqMax = dataGridAllDataObserver.listGroup[i].freqMax;
                        newGroupBuf.timeGoOn = dataGridAllDataObserver.listGroup[i].timeGoOn;
                        newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                        newGroupBuf.freqMin = dataGridAllDataObserver.listGroup[i].freqMin;
                        newGroupBuf.timeGoOff = dataGridAllDataObserver.listGroup[i].timeGoOff;
                        newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                        newGroupBuf.cycle = dataGridAllDataObserver.listGroup[i].timeGoOn;
                    }
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }
        private void GroupTest_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listGroup_DataGrid.SelectedIndex;
                //MessageBox.Show(index.ToString());
                if (dataGridAllDataObserver.listGroup[index].id == hardwareConfig.listGroup[index].groupID)
                {
                    hardwareConfig.listGroup[index].effectRunning = dataGridAllDataObserver.listGroup[index].effect;
                    hardwareConfig.listGroup[index].freqMax = dataGridAllDataObserver.listGroup[index].freqMax;
                    hardwareConfig.listGroup[index].timeGoOn = dataGridAllDataObserver.listGroup[index].timeGoOn;
                    hardwareConfig.listGroup[index].timeGoOff = dataGridAllDataObserver.listGroup[index].timeGoOff;
                    hardwareConfig.listGroup[index].freqMin = dataGridAllDataObserver.listGroup[index].freqMin;
                    hardwareConfig.listGroup[index].timeHoldOn = dataGridAllDataObserver.listGroup[index].timeHoldOn;
                    hardwareConfig.listGroup[index].timeHoldOff = dataGridAllDataObserver.listGroup[index].timeHoldOff;
                    hardwareConfig.listGroup[index].cycle = dataGridAllDataObserver.listGroup[index].timeHoldOn;
                    testClickEvent.Invoke(this, hardwareConfig.listGroup[index]);
                }
            }
            catch
            {
                MessageBox.Show("Vui lòng lưu thông tin cấu hình nhóm trước khi gửi");
            }
        }
        
        
    }
}
