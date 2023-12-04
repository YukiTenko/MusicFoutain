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

namespace MusicFountain.UI.Hardware.Group
{
    /// <summary>
    /// Interaction logic for UI_Group_LEDDMX_Artnet.xaml
    /// </summary>
    public partial class UI_Group_LEDDMX_Artnet : Page
    {
        // Support class
        private class DataGrid_GroupConfig
        {
            public int id { get; set; }
            public string name { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string effect { get; set; }
            public int timeHoldOn { get; set; }
            public int timeHoldOff { get; set; }
            public int red { get; set; }
            public int green { get; set; }
            public int blue { get; set; }
        }
        private class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_GroupConfig> listGroup { get; set; }
        }

        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver = new DataGrid_AllData_Observer();
        // For backend data
        private Data.Data_Config_Hardware hardwareConfig;

        // Events
        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;
        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        public UI_Group_LEDDMX_Artnet()
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
                    EffectListConverter effectListBuf = new EffectListConverter();
                    groupEffect_ComboBox.ItemsSource = effectListBuf.LIST_EFFECT_LED;

                    listGroup_DataGrid.ItemsSource = dataGridAllDataObserver.listGroup;
                    CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                    List<string> listLEDBuf = new List<string>();
                    for (int i = 0; i < 512; i++)
                    {
                        string Led = "Led " + (i + 1).ToString();
                        listLEDBuf.Add(Led);
                    }
                    devStart_ComboBox.ItemsSource = listLEDBuf;
                    devEnd_ComboBox.ItemsSource = listLEDBuf;
                    CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
                }
            }
        }

        // Private methods
        private void GroupTest_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listGroup_DataGrid.SelectedIndex;
                //MessageBox.Show(index.ToString());
                if (dataGridAllDataObserver.listGroup[index].id == hardwareConfig.listGroup[index].groupID)
                {
                    hardwareConfig.listGroup[index].effectRunning = dataGridAllDataObserver.listGroup[index].effect;
                    hardwareConfig.listGroup[index].timeHoldOn = dataGridAllDataObserver.listGroup[index].timeHoldOn;
                    hardwareConfig.listGroup[index].timeHoldOff = dataGridAllDataObserver.listGroup[index].timeHoldOff;
                    hardwareConfig.listGroup[index].red = dataGridAllDataObserver.listGroup[index].red;
                    hardwareConfig.listGroup[index].green = dataGridAllDataObserver.listGroup[index].green;
                    hardwareConfig.listGroup[index].blue = dataGridAllDataObserver.listGroup[index].blue;
                    testClickEvent.Invoke(this, hardwareConfig.listGroup[index]);
                }
            }
            catch
            {
                MessageBox.Show("Vui lòng lưu thông tin cấu hình nhóm trước khi gửi");
            }
        }
        private void GroupSave_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            if (updateClickEvent != null)
            {
                Data.Data_Config_Hardware newHardwareGroupInfo = new  Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listLED_ArtnetDMX = hardwareConfig.listLED_ArtnetDMX;
                newHardwareGroupInfo.optionLED_ArtnetDMX = hardwareConfig.optionLED_ArtnetDMX;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < dataGridAllDataObserver.listGroup.Count; i++)
                {
                     Data.Data_Config_Hardware_Group newGroupBuf = new  Data.Data_Config_Hardware_Group();
                    newGroupBuf.hardwareID = newHardwareGroupInfo.hardwareID;
                    newGroupBuf.groupID = dataGridAllDataObserver.listGroup[i].id;
                    newGroupBuf.name = dataGridAllDataObserver.listGroup[i].name;
                    newGroupBuf.type = newHardwareGroupInfo.type;
                    try
                    {
                        newGroupBuf.startDevID = int.Parse(dataGridAllDataObserver.listGroup[i].startDev.Replace("Led ", ""));
                        newGroupBuf.endDevID = int.Parse(dataGridAllDataObserver.listGroup[i].endDev.Replace("Led ", ""));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Vui lòng chọn danh sách thiết bị trong nhóm trước khi lưu . Hiên tại sẽ lưu danh sách thiết bị mặc định .");
                        newGroupBuf.startDevID = 1;
                        newGroupBuf.endDevID = 4;
                    }
                    newGroupBuf.effectSave = dataGridAllDataObserver.listGroup[i].effect;
                    newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                    newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                    newGroupBuf.red = dataGridAllDataObserver.listGroup[i].red;
                    newGroupBuf.green = dataGridAllDataObserver.listGroup[i].green;
                    newGroupBuf.blue = dataGridAllDataObserver.listGroup[i].blue;
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }
        
        // Public methods
        public void Update_HardwareConfig( Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            dataGridAllDataObserver.listGroup.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                DataGrid_GroupConfig buffer1 = new DataGrid_GroupConfig();
                buffer1.id = hardwareConfig.listGroup[i].groupID;
                buffer1.name = hardwareConfig.listGroup[i].name;
                buffer1.startDev = "Led " + hardwareConfig.listGroup[i].startDevID.ToString();
                buffer1.endDev = "Led " + hardwareConfig.listGroup[i].endDevID.ToString();
                buffer1.effect = hardwareConfig.listGroup[i].effectRunning;
                buffer1.timeHoldOn = (int)hardwareConfig.listGroup[i].timeHoldOn;
                buffer1.timeHoldOff = (int)hardwareConfig.listGroup[i].timeHoldOff;
                buffer1.red = hardwareConfig.listGroup[i].red;
                buffer1.green = hardwareConfig.listGroup[i].green;
                buffer1.blue = hardwareConfig.listGroup[i].blue;
                dataGridAllDataObserver.listGroup.Add(buffer1);
            }
            CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
        }
    }
}
