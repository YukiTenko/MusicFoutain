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
    /// Interaction logic for UI_Log.xaml
    /// </summary>
    public partial class UI_Log : Page
    {
        // Variables
        //private int currentLogNumber;
        //private int maxLogNumber;
        //private int numberLogInPage;
        private int currentLogPage;
        //private int maxLogPage;

        // UI
        private Button backButton;
        private Button prevButton;
        private Button nextButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<int> prevClick;
        public event EventHandler<int> nextClick;

        public UI_Log(string xmlFilePath)
        {
            InitializeComponent();

            {// Creat variables
                //currentLogNumber = 0;
                //maxLogNumber = 0;
                //numberLogInPage = 0;
                currentLogPage = 0;
                //maxLogPage = 0;
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_Log: Can not find back button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }
                prevButton = LogicalTreeHelper.FindLogicalNode(rootObject, "prev") as Button;
                if (prevButton is null)
                {
                    MessageBox.Show("UI_Log: Can not find prev button in file " + xmlFilePath);
                }
                else
                {
                    prevButton.Click += new RoutedEventHandler(PrevButton_Click);
                }
                nextButton = LogicalTreeHelper.FindLogicalNode(rootObject, "next") as Button;
                if (nextButton is null)
                {
                    MessageBox.Show("UI_Log: Can not find next button in file " + xmlFilePath);
                }
                else
                {
                    nextButton.Click += new RoutedEventHandler(NextButton_Click);
                }

                mainLayout.Children.Add(rootObject);
            }
            catch
            {
                MessageBox.Show("UI_Log: Can not load file " + xmlFilePath);
            }

        }
        // Private methods -------------------------------------------------------------------------------
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            backClick.Invoke(this, 0);
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            prevClick.Invoke(this, currentLogPage);
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            nextClick.Invoke(this, currentLogPage);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
