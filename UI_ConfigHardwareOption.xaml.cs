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
    /// <summary>
    /// Interaction logic for UI_ConfigHardwareOption.xaml
    /// </summary>
    public partial class UI_ConfigHardwareOption : Page
    {
        // Variables

        // UI
        private Button backButton;
        private Button saveButton;
        private TextBox vsaMaxAngleTextBox;
        private TextBox vsaMaxSpeedTextBox;
        private TextBox vsaCoordAngleTextBox;
        private TextBox vsaRatioTextBox;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<int> saveClick;
        public UI_ConfigHardwareOption(string xmlFilePath)
        {
            InitializeComponent();

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                saveButton = LogicalTreeHelper.FindLogicalNode(rootObject, "save") as Button;
                if (saveButton is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find save Button in file " + xmlFilePath);
                }
                else
                {
                    saveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }

                vsaMaxAngleTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaMaxAngle") as TextBox;
                if (vsaMaxAngleTextBox is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find vsaMaxAngle TextBox in file " + xmlFilePath);
                }

                vsaMaxSpeedTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaMaxSpeed") as TextBox;
                if (vsaMaxSpeedTextBox is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find vsaMaxSpeed TextBox in file " + xmlFilePath);
                }

                vsaCoordAngleTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaCoordAngle") as TextBox;
                if (vsaCoordAngleTextBox is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find vsaCoordAngle TextBox in file " + xmlFilePath);
                }

                vsaRatioTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaRatio") as TextBox;
                if (vsaRatioTextBox is null)
                {
                    MessageBox.Show("UI_ConfigHardwareOption: Can not find vsaRatio TextBox in file " + xmlFilePath);
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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            saveClick.Invoke(this, 0);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
