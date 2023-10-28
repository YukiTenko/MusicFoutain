using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicFountain
{
    public class UISupport_GroupEdit_VS_PLC
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public float setSpeed { get; set; }
        public float setAngle { get; set; }
    }
    /// <summary>
    /// Interaction logic for UI_GroupEdit_VS_PLC.xaml
    /// </summary>
    public partial class UI_GroupEdit_VS_PLC : Page
    {
        // Variables
        private List<UISupport_GroupEdit_VS_PLC> listVSA;

        // UI
        private Button backButton;

        // Event
        public event EventHandler<int> backClick;
        public UI_GroupEdit_VS_PLC(string xmlFilePath)
        {
            InitializeComponent();

            {// Variable creat
                listVSA = new List<UISupport_GroupEdit_VS_PLC>();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigGroup: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                mainLayout.Children.Add(rootObject);
            }
            catch
            {
                MessageBox.Show("UI_Running: Can not load file " + xmlFilePath);
            }
        }
        // Private methods -------------------------------------------------------------------------------
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            backClick.Invoke(this, 0);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
