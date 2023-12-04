using System;
using System.Collections.Generic;
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

namespace MusicFountain.UI.Hardware.Setting
{
    /// <summary>
    /// Interaction logic for UI_Setting_PumpAD_PLC.xaml
    /// </summary>
    public partial class UI_Setting_PumpAD_PLC : Page
    {
        // Variables
         Data.Data_Config_Hardware hardwareConfig;

        // Events
        public event EventHandler<Data.Data_Config_Hardware> updateHardwareOption;
        public UI_Setting_PumpAD_PLC()
        {
            InitializeComponent();

            {// Creat variables
                hardwareConfig = null;
            }
        }

        private void Update_Inverter_Option(object sender, RoutedEventArgs e)
        {
            hardwareConfig.optionInverter.freqMax = float.Parse(freqMax.Text);
            hardwareConfig.optionInverter.freqMin = float.Parse(freqMin.Text);
            updateHardwareOption.Invoke(this, hardwareConfig);
        }

        public void UpdateHardwareConfig( Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            hardwareName.Content = "Cấu hình tham số cho " + hardwareConfig.name;
            freqMax.Text = hardwareConfig.optionInverter.freqMax.ToString();
            freqMin.Text = hardwareConfig.optionInverter.freqMin.ToString();
        }
    }
}
