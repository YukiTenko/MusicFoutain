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
    /// Interaction logic for UI_Running.xaml
    /// </summary>
    public partial class UI_Running : Page
    {
        // Variables

        // UI
        private Button backButton;
        private Button configGroupButton;
        private Button configHardwareButton;
        private Button editPlaylistButton;
        private Button logButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<int> configGroupClick;
        public event EventHandler<int> configHardwareClick;
        public event EventHandler<int> editPlaylistClick;
        public event EventHandler<int> logClick;
        public UI_Running(string xmlFilePath)
        {
            InitializeComponent();

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_Running: Can not find back button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                configGroupButton = LogicalTreeHelper.FindLogicalNode(rootObject, "configGroup") as Button;
                if (configGroupButton is null)
                {
                    MessageBox.Show("UI_Running: Can not find config group button in file " + xmlFilePath);
                }
                else
                {
                    configGroupButton.Click += new RoutedEventHandler(ConfigGroupButton_Click);
                }

                configHardwareButton = LogicalTreeHelper.FindLogicalNode(rootObject, "configHardware") as Button;
                if (configHardwareButton is null)
                {
                    MessageBox.Show("UI_Running: Can not find config hardware button in file " + xmlFilePath);
                }
                else
                {
                    configHardwareButton.Click += new RoutedEventHandler(ConfigHardwareButton_Click);
                }

                editPlaylistButton = LogicalTreeHelper.FindLogicalNode(rootObject, "editPlaylist") as Button;
                if (editPlaylistButton is null)
                {
                    MessageBox.Show("UI_Running: Can not find edit playlist button in file " + xmlFilePath);
                }
                else
                {
                    editPlaylistButton.Click += new RoutedEventHandler(EditPlaylistButton_Click);
                }

                logButton = LogicalTreeHelper.FindLogicalNode(rootObject, "log") as Button;
                if (logButton is null)
                {
                    MessageBox.Show("UI_Running: Can not find log button in file " + xmlFilePath);
                }
                else
                {
                    logButton.Click += new RoutedEventHandler(LogButton_Click);
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
        private void ConfigGroupButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            configGroupClick.Invoke(this, 0);
        }
        private void ConfigHardwareButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            configHardwareClick.Invoke(this, 0);
        }
        private void EditPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            editPlaylistClick.Invoke(this, 0);
        }
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            logClick.Invoke(this, 0);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
