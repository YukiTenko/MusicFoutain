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
using System.Windows.Threading;
using System.Timers;
using System.Threading;
using ScottPlot.Plottable;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Color = System.Drawing.Color;
using System.IO;
using System.Collections;

namespace MusicFountain.UI.Music
{
    public partial class UI_MusicEditor : Page
    {
        // Support class
        private class DataGrid_Time
        {
            public int idTime { get; set; }
            public string timeStart { get; set; }
        }
        private class DataGrid_MusicEffect
        {
            public int id { get; set; }
            public int idTime { get; set; }
            public double timeStart { get; set; }
            public double timeStop { get; set; }
            public double timeLength { get; set; }
            public int effectID { get; set; }
            public string effectName { get; set; }
            public int hardwareID { get; set; }
            public int groupID { get; set; }
            public string groupName { get; set; }
            public string groupType { get; set; }
            public string groupEffect { get; set; }
            public string groupPara1Name { get; set; }
            public string groupPara1 { get; set; }
            public string groupPara2Name { get; set; }
            public string groupPara2 { get; set; }
            public string groupPara3Name { get; set; }
            public string groupPara3 { get; set; }
            public string groupPara4Name { get; set; }
            public string groupPara4 { get; set; }
            public string groupPara5Name { get; set; }
            public string groupPara5 { get; set; }
            public string groupPara6Name { get; set; }
            public string groupPara6 { get; set; }
        }
        private class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_MusicEffect> listMusicEffect;
            public ObservableCollection<DataGrid_Time> listTime;
        }
        public class Grid_TimeLine
        {
            // Support class
            public class EffectTimeLine
            {
                public string effectName;
                public int xPos;
                public int timeLength;
                public SolidColorBrush specialColor;

            }

            // Variables
            public List<EffectTimeLine> listEffectTimeLine;
            public string headerName;
            public int groupID;
            public int hardwareID;

            public Grid_TimeLine()
            {
                listEffectTimeLine = new List<EffectTimeLine>();
            }
        }

        // Constants
        private const int ZOOM_CONSTAN = 10;
        private SolidColorBrush[] ARRAY_EFFECT_COLORR = { Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkCyan, Brushes.DarkGreen, Brushes.DarkSlateGray, Brushes.DarkMagenta};
        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
        // For backend data
        private List<string> listMusicName;
        private List<string> listGroupName;
        private List<string> listEffectName;
        private List<int> listTimeID;
        private List<Data.Data_Config_Music> listMusic;
        private List<Data.Data_Config_Hardware_Group> listGroup;
        private List<Data.Data_Config_Effect> listEffect;
        // For play media
        private MediaPlayer mediaPlayer;
        private bool mediaPlayerState;
        private VLine vLine;
        private List<Grid_TimeLine> listTimeLine;
        // For music timer
        private DispatcherTimer supportTimer;
        private Thread updateMusicTimeThread;
        private bool updateMusicTimeThreadRunning;
        private double musicTime;
        private double realTime;
        private double readTimePre;

        // Events
        public event EventHandler<Data.Data_Config_Music> playMusicClick;
        //public event EventHandler<int> pauseMusicClick;
        public event EventHandler<int> stopMusicClick;
        public event EventHandler<double> playMusicTimeIntervalUpdate;
        public event EventHandler<Data.Data_Config_Music> saveMusicEffectClick;

        public UI_MusicEditor()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listTime = new ObservableCollection<DataGrid_Time>();
                    dataGridAllDataObserver.listMusicEffect = new ObservableCollection<DataGrid_MusicEffect>();
                }
                {// For backend process
                    listMusicName = new List<string>();
                    listGroupName = new List<string>();
                    listEffectName = new List<string>();
                    listMusic = new List<Data.Data_Config_Music>();
                    listGroup = new List<Data.Data_Config_Hardware_Group>();
                    listEffect = new List<Data.Data_Config_Effect>();
                    listTimeID = new List<int>();

                    string[] listMusicFilesBuf = Directory.GetFiles("../Media");
                    listMusicName.Clear();
                    foreach (string musicFileBuf in listMusicFilesBuf)
                    {
                        listMusicName.Add(musicFileBuf);
                    }
                    listTimeLine = new List<Grid_TimeLine>();

                    mediaPlayerState = false;
                    updateMusicTimeThreadRunning = false;
                }
            }

            {// Media and time creat
                mediaPlayer = new MediaPlayer();
                mediaPlayer.MediaEnded += MediaEnded_EventHandle;
                mediaPlayer.MediaFailed += MediaFailed_EventHandle;
                vLine = new VLine();

                supportTimer = new DispatcherTimer();
                supportTimer.Interval = TimeSpan.FromMilliseconds(500);
                supportTimer.Tick += SupportTimer_Tick;
                supportTimer.Start();

                updateMusicTimeThread = new Thread(Update_MusicTime_Thread);
            }

            {// First time UI Update
                {// Set source for data grid first
                    listTime_DataGrid.ItemsSource = dataGridAllDataObserver.listTime;
                    CollectionViewSource.GetDefaultView(listTime_DataGrid.ItemsSource).Refresh();

                    listMusicEffect_DataGrid.ItemsSource = dataGridAllDataObserver.listMusicEffect;
                    CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                    musicSelection_ComboBox.ItemsSource = listMusicName;
                    if (musicSelection_ComboBox.Items.Count > 0)
                    {
                        musicSelection_ComboBox.SelectedIndex = 0;
                    }
                }
            }
        }
        // Private methods
        private void MusicSelection_ComboBoxChanged_EventHandle(object sender, SelectionChangedEventArgs e)
        {
            if (musicSelection_ComboBox.SelectedItem.ToString() != "")
            {// Change config of music effect
             // Find music config in list music
                Data.Data_Config_Music newMusicConfig = new Data.Data_Config_Music();
                newMusicConfig.listEffect = new List<Data.Data_Config_Effect_With_Time>();
                for (int i = 0; i < listMusic.Count; i++)
                {
                    if (musicSelection_ComboBox.SelectedItem.ToString() == listMusic[i].musicPath)
                    {
                        newMusicConfig = listMusic[i];
                    }
                }
                // Update config of music
                dataGridAllDataObserver.listTime.Clear();
                dataGridAllDataObserver.listMusicEffect.Clear();
                for (int i = 0; i < newMusicConfig.listEffect.Count; i++)
                {
                    DataGrid_Time newDataGrid_TimeBuf = new DataGrid_Time();
                    newDataGrid_TimeBuf.idTime = newMusicConfig.listEffect[i].musicTimeID;
                    newDataGrid_TimeBuf.timeStart = newMusicConfig.listEffect[i].musicTimeStart.ToString();
                    dataGridAllDataObserver.listTime.Add(newDataGrid_TimeBuf);
                    for (int j = 0; j < newMusicConfig.listEffect[i].listEffectData.Count; j++)
                    {
                        DataGrid_MusicEffect dataGridMusicEffectBuf = new DataGrid_MusicEffect();
                        dataGridMusicEffectBuf.idTime = newMusicConfig.listEffect[i].musicTimeID;
                        dataGridMusicEffectBuf.timeLength = newMusicConfig.listEffect[i].musicTimeLength;
                        dataGridMusicEffectBuf.timeStart = newMusicConfig.listEffect[i].musicTimeStart;
                        dataGridMusicEffectBuf.timeStop = newMusicConfig.listEffect[i].musicTimeStop;

                        dataGridMusicEffectBuf.effectID = newMusicConfig.listEffect[i].listEffectData[j].effectID;
                        dataGridMusicEffectBuf.effectName = newMusicConfig.listEffect[i].listEffectData[j].effectName;
                        dataGridMusicEffectBuf.hardwareID = newMusicConfig.listEffect[i].listEffectData[j].hardwareID;
                        dataGridMusicEffectBuf.groupID = newMusicConfig.listEffect[i].listEffectData[j].groupID;
                        dataGridMusicEffectBuf.groupName = newMusicConfig.listEffect[i].listEffectData[j].groupName;
                        dataGridMusicEffectBuf.groupEffect = newMusicConfig.listEffect[i].listEffectData[j].groupEffect;

                        dataGridMusicEffectBuf.groupPara1 = newMusicConfig.listEffect[i].listEffectData[j].para1;
                        dataGridMusicEffectBuf.groupPara2 = newMusicConfig.listEffect[i].listEffectData[j].para2;
                        dataGridMusicEffectBuf.groupPara3 = newMusicConfig.listEffect[i].listEffectData[j].para3;
                        dataGridMusicEffectBuf.groupPara4 = newMusicConfig.listEffect[i].listEffectData[j].para4;
                        dataGridMusicEffectBuf.groupPara5 = newMusicConfig.listEffect[i].listEffectData[j].para5;
                        dataGridMusicEffectBuf.groupPara6 = newMusicConfig.listEffect[i].listEffectData[j].para6;

                        dataGridAllDataObserver.listMusicEffect.Add(dataGridMusicEffectBuf);
                    }
                }
                {// Update listTimeID to combobox
                    listTimeID.Clear();
                    for (int i = 0; i < dataGridAllDataObserver.listTime.Count; i++)
                    {
                        listTimeID.Add(dataGridAllDataObserver.listTime[i].idTime);
                    }
                    timeID_ComboBox.ItemsSource = listTimeID;
                }
                for (int i = 0; i < dataGridAllDataObserver.listMusicEffect.Count; i++)
                {
                    dataGridAllDataObserver.listMusicEffect[i].id = i + 1;
                    for (int j = 0; j < listEffect.Count; j++)
                    {
                        if (dataGridAllDataObserver.listMusicEffect[i].effectName == listEffect[j].effectName)
                        {
                            dataGridAllDataObserver.listMusicEffect[i].effectID = listEffect[j].effectID;
                            dataGridAllDataObserver.listMusicEffect[i].effectName = listEffect[j].effectName;
                            dataGridAllDataObserver.listMusicEffect[i].hardwareID = listEffect[j].hardwareID;
                            dataGridAllDataObserver.listMusicEffect[i].groupID = listEffect[j].groupID;
                            dataGridAllDataObserver.listMusicEffect[i].groupType = listEffect[j].groupType;
                            dataGridAllDataObserver.listMusicEffect[i].groupName = listEffect[j].groupName;

                            dataGridAllDataObserver.listMusicEffect[i].groupPara1Name = listEffect[j].para1Name;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara2Name = listEffect[j].para2Name;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara3Name = listEffect[j].para3Name;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara4Name = listEffect[j].para4Name;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara5Name = listEffect[j].para5Name;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara6Name = listEffect[j].para6Name;

                            dataGridAllDataObserver.listMusicEffect[i].groupPara1 = listEffect[j].para1;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara2 = listEffect[j].para2;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara3 = listEffect[j].para3;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara4 = listEffect[j].para4;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara5 = listEffect[j].para5;
                            dataGridAllDataObserver.listMusicEffect[i].groupPara6 = listEffect[j].para6;
                        }
                    }
                }
                CollectionViewSource.GetDefaultView(listTime_DataGrid.ItemsSource).Refresh();
                CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
                // Update wpfPlot and timeline
                mediaPlayer.Open(new System.Uri(System.IO.Path.GetFullPath(musicSelection_ComboBox.SelectedItem.ToString())));
                AudioFileReader stream;
                stream = new AudioFileReader(musicSelection_ComboBox.SelectedItem.ToString());
                WpfPlot_Initialize(stream);
                Refresh_GridTimeLine((int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        private void UpdateListTime_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            bool timeStartCheck = true;
            {// Creat time ID and check time large than pretime
                for (int i = 0; i < dataGridAllDataObserver.listTime.Count; i++)
                {
                    dataGridAllDataObserver.listTime[i].idTime = i + 1;
                    try
                    {
                        if (int.Parse(dataGridAllDataObserver.listTime[i].timeStart) > mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds)
                        {
                            timeStartCheck = false;
                            i = dataGridAllDataObserver.listTime.Count;
                        }
                        if (i > 0)
                        {
                            if ((int.Parse(dataGridAllDataObserver.listTime[i].timeStart) - int.Parse(dataGridAllDataObserver.listTime[i - 1].timeStart)) < 1000)
                            {
                                timeStartCheck = false;
                                i = dataGridAllDataObserver.listTime.Count;
                            }
                        }
                        else
                        {
                            if (int.Parse(dataGridAllDataObserver.listTime[i].timeStart) < 0)
                            {
                                timeStartCheck = false;
                                i = dataGridAllDataObserver.listTime.Count;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                CollectionViewSource.GetDefaultView(listTime_DataGrid.ItemsSource).Refresh();
            }
            if (timeStartCheck) {// Update listTimeID to combobox
                listTimeID.Clear();
                for (int i = 0; i < dataGridAllDataObserver.listTime.Count; i++)
                {
                    listTimeID.Add(dataGridAllDataObserver.listTime[i].idTime);
                }
                timeID_ComboBox.ItemsSource = listTimeID;
                CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
                MessageBox.Show("Cập nhập các mốc thời gian thành công");
            }
            else
            {
                MessageBox.Show("Vui lòng nhập lại các mốc thời gian theo những yêu cầu sau \n\tLà số nguyên dương \n\tCó giá trị tăng dần và cách nhau tối thiểu 1000 ms \n\tNhỏ hơn độ dài bài hát");
            }
        }
        private void UpdateListTimeAndEffectOrder_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dataGridAllDataObserver.listMusicEffect.Count; i++)
            {
                dataGridAllDataObserver.listMusicEffect[i].id = i + 1;
                for (int j = 0; j < listEffect.Count; j++)
                {
                    if (dataGridAllDataObserver.listMusicEffect[i].effectName == listEffect[j].effectName)
                    {
                        dataGridAllDataObserver.listMusicEffect[i].effectID = listEffect[j].effectID;
                        dataGridAllDataObserver.listMusicEffect[i].effectName = listEffect[j].effectName;
                        dataGridAllDataObserver.listMusicEffect[i].hardwareID = listEffect[j].hardwareID;
                        dataGridAllDataObserver.listMusicEffect[i].groupID = listEffect[j].groupID;
                        dataGridAllDataObserver.listMusicEffect[i].groupType = listEffect[j].groupType;
                        dataGridAllDataObserver.listMusicEffect[i].groupName = listEffect[j].groupName;
                        dataGridAllDataObserver.listMusicEffect[i].groupEffect = listEffect[j].groupEffect;

                        dataGridAllDataObserver.listMusicEffect[i].groupPara1Name = listEffect[j].para1Name;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara2Name = listEffect[j].para2Name;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara3Name = listEffect[j].para3Name;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara4Name = listEffect[j].para4Name;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara5Name = listEffect[j].para5Name;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara6Name = listEffect[j].para6Name;

                        dataGridAllDataObserver.listMusicEffect[i].groupPara1 = listEffect[j].para1;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara2 = listEffect[j].para2;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara3 = listEffect[j].para3;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara4 = listEffect[j].para4;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara5 = listEffect[j].para5;
                        dataGridAllDataObserver.listMusicEffect[i].groupPara6 = listEffect[j].para6;
                    }
                }
            }
            CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
            MessageBox.Show("Cập nhập thông tin hiệu ứng thành công");
        }
        private void SaveListEffect_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            Data.Data_Config_Music musicConfigBuf = new Data.Data_Config_Music();
            musicConfigBuf.musicPath = musicSelection_ComboBox.SelectedItem.ToString();
            musicConfigBuf.listEffect = new List<Data.Data_Config_Effect_With_Time>();
            musicConfigBuf.listEffect.Clear();
            for (int i = 0; i < dataGridAllDataObserver.listMusicEffect.Count; i++)
            {
                bool checkNewTimeID = true;

                for (int j = 0; j < musicConfigBuf.listEffect.Count; j++)
                {
                    if (dataGridAllDataObserver.listMusicEffect[i].idTime == musicConfigBuf.listEffect[j].musicTimeID)
                    {
                        checkNewTimeID = false;
                        Data.Data_Config_Effect newgroupDataBuf = new Data.Data_Config_Effect();
                        newgroupDataBuf.effectID = dataGridAllDataObserver.listMusicEffect[i].effectID;
                        newgroupDataBuf.effectName = dataGridAllDataObserver.listMusicEffect[i].effectName;
                        newgroupDataBuf.hardwareID = dataGridAllDataObserver.listMusicEffect[i].hardwareID;
                        newgroupDataBuf.groupID = dataGridAllDataObserver.listMusicEffect[i].groupID;
                        newgroupDataBuf.groupName = dataGridAllDataObserver.listMusicEffect[i].groupName;
                        newgroupDataBuf.groupType = dataGridAllDataObserver.listMusicEffect[i].groupType;
                        newgroupDataBuf.groupEffect = dataGridAllDataObserver.listMusicEffect[i].groupEffect;
                        for (int k = 0; k < listEffect.Count; k++)
                        {
                            if ((newgroupDataBuf.hardwareID == listEffect[k].hardwareID) && (newgroupDataBuf.groupID == listEffect[k].groupID) && (newgroupDataBuf.effectID == listEffect[k].effectID))
                            {
                                newgroupDataBuf.listInverter = listEffect[k].listInverter;
                                newgroupDataBuf.listVO_PLC = listEffect[k].listVO_PLC;
                                newgroupDataBuf.listVS_PLC = listEffect[k].listVS_PLC;
                                newgroupDataBuf.listLED_ArtnetDMX = listEffect[k].listLED_ArtnetDMX;
                                newgroupDataBuf.listLED_PLC = listEffect[k].listLED_PLC;
                            }
                        }
                        newgroupDataBuf.para1 = dataGridAllDataObserver.listMusicEffect[i].groupPara1;
                        newgroupDataBuf.para2 = dataGridAllDataObserver.listMusicEffect[i].groupPara2;
                        newgroupDataBuf.para3 = dataGridAllDataObserver.listMusicEffect[i].groupPara3;
                        newgroupDataBuf.para4 = dataGridAllDataObserver.listMusicEffect[i].groupPara4;
                        newgroupDataBuf.para5 = dataGridAllDataObserver.listMusicEffect[i].groupPara5;
                        newgroupDataBuf.para6 = dataGridAllDataObserver.listMusicEffect[i].groupPara6;
                        musicConfigBuf.listEffect[j].listEffectData.Add(newgroupDataBuf);
                    }
                }

                if (checkNewTimeID)
                {
                    Data.Data_Config_Effect_With_Time newListEffectWithTimeBuf = new Data.Data_Config_Effect_With_Time();
                    newListEffectWithTimeBuf.musicTimeID = dataGridAllDataObserver.listMusicEffect[i].idTime;
                    for (int j = 0; j < dataGridAllDataObserver.listTime.Count; j++)
                    {
                        if (newListEffectWithTimeBuf.musicTimeID == dataGridAllDataObserver.listTime[j].idTime)
                        {
                            newListEffectWithTimeBuf.musicTimeStart = double.Parse(dataGridAllDataObserver.listTime[j].timeStart);
                            if (newListEffectWithTimeBuf.musicTimeID < (dataGridAllDataObserver.listTime.Count - 1))
                            {
                                newListEffectWithTimeBuf.musicTimeStop = double.Parse(dataGridAllDataObserver.listTime[j + 1].timeStart);
                                newListEffectWithTimeBuf.musicTimeLength = newListEffectWithTimeBuf.musicTimeStop - newListEffectWithTimeBuf.musicTimeStart;
                            }
                            else
                            {
                                newListEffectWithTimeBuf.musicTimeStop = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                                newListEffectWithTimeBuf.musicTimeLength = newListEffectWithTimeBuf.musicTimeStop - newListEffectWithTimeBuf.musicTimeStart;
                            }
                        }
                    }

                    newListEffectWithTimeBuf.listEffectData = new List<Data.Data_Config_Effect>();
                    newListEffectWithTimeBuf.listEffectData.Clear();
                    Data.Data_Config_Effect newgroupDataBuf = new Data.Data_Config_Effect();
                    newgroupDataBuf.effectID = dataGridAllDataObserver.listMusicEffect[i].effectID;
                    newgroupDataBuf.effectName = dataGridAllDataObserver.listMusicEffect[i].effectName;
                    newgroupDataBuf.hardwareID = dataGridAllDataObserver.listMusicEffect[i].hardwareID;
                    newgroupDataBuf.groupID = dataGridAllDataObserver.listMusicEffect[i].groupID;
                    newgroupDataBuf.groupName = dataGridAllDataObserver.listMusicEffect[i].groupName;
                    newgroupDataBuf.groupType = dataGridAllDataObserver.listMusicEffect[i].groupType;
                    newgroupDataBuf.groupEffect = dataGridAllDataObserver.listMusicEffect[i].groupEffect;
                    for (int k = 0; k < listEffect.Count; k++)
                    {
                        if ((newgroupDataBuf.hardwareID == listEffect[k].hardwareID) && (newgroupDataBuf.groupID == listEffect[k].groupID))
                        {
                            newgroupDataBuf.listInverter = listEffect[k].listInverter;
                            newgroupDataBuf.listVO_PLC = listEffect[k].listVO_PLC;
                            newgroupDataBuf.listVS_PLC = listEffect[k].listVS_PLC;
                            newgroupDataBuf.listLED_ArtnetDMX = listEffect[k].listLED_ArtnetDMX;
                            newgroupDataBuf.listLED_PLC = listEffect[k].listLED_PLC;
                        }
                    }
                    newgroupDataBuf.para1 = dataGridAllDataObserver.listMusicEffect[i].groupPara1;
                    newgroupDataBuf.para2 = dataGridAllDataObserver.listMusicEffect[i].groupPara2;
                    newgroupDataBuf.para3 = dataGridAllDataObserver.listMusicEffect[i].groupPara3;
                    newgroupDataBuf.para4 = dataGridAllDataObserver.listMusicEffect[i].groupPara4;
                    newgroupDataBuf.para5 = dataGridAllDataObserver.listMusicEffect[i].groupPara5;
                    newgroupDataBuf.para6 = dataGridAllDataObserver.listMusicEffect[i].groupPara6;

                    newListEffectWithTimeBuf.listEffectData.Add(newgroupDataBuf);
                    musicConfigBuf.listEffect.Add(newListEffectWithTimeBuf);
                }
            }
            saveMusicEffectClick.Invoke(this, musicConfigBuf);
        }

        private void Play_ButtonClick_EventHanle(object sender, RoutedEventArgs e)
        {
            if (musicSelection_ComboBox.SelectedItem.ToString() != "")
            {// Change music
                if (mediaPlayerState)
                {
                    MessageBox.Show("Một bản nhạc đã đang chạy\nVui lòng dừng bản nhạc đang chạy trước khi phát");
                }
                else
                {
                    mediaPlayer.Play();
                    mediaPlayerState = true;
                    musicTime_Slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

                    {// Send list effect to control thread
                        Data.Data_Config_Music musicConfigBuf = new Data.Data_Config_Music();
                        musicConfigBuf.musicPath = musicSelection_ComboBox.SelectedItem.ToString();
                        musicConfigBuf.listEffect = new List<Data.Data_Config_Effect_With_Time>();
                        musicConfigBuf.listEffect.Clear();
                        for (int i = 0; i < dataGridAllDataObserver.listMusicEffect.Count; i++)
                        {
                            bool checkNewTimeID = true;

                            for (int j = 0; j < musicConfigBuf.listEffect.Count; j++)
                            {
                                if (dataGridAllDataObserver.listMusicEffect[i].idTime == musicConfigBuf.listEffect[j].musicTimeID)
                                {
                                    checkNewTimeID = false;
                                    Data.Data_Config_Effect newgroupDataBuf = new Data.Data_Config_Effect();
                                    newgroupDataBuf.effectID = dataGridAllDataObserver.listMusicEffect[i].effectID;
                                    newgroupDataBuf.effectName = dataGridAllDataObserver.listMusicEffect[i].effectName;
                                    newgroupDataBuf.hardwareID = dataGridAllDataObserver.listMusicEffect[i].hardwareID;
                                    newgroupDataBuf.groupID = dataGridAllDataObserver.listMusicEffect[i].groupID;
                                    newgroupDataBuf.groupName = dataGridAllDataObserver.listMusicEffect[i].groupName;
                                    newgroupDataBuf.groupType = dataGridAllDataObserver.listMusicEffect[i].groupType;
                                    newgroupDataBuf.groupEffect = dataGridAllDataObserver.listMusicEffect[i].groupEffect;
                                    for (int k = 0; k < listEffect.Count; k++)
                                    {
                                        if ((newgroupDataBuf.hardwareID == listEffect[k].hardwareID) && (newgroupDataBuf.groupID == listEffect[k].groupID) && (newgroupDataBuf.effectID == listEffect[k].effectID))
                                        {
                                            newgroupDataBuf.listInverter = listEffect[k].listInverter;
                                            newgroupDataBuf.listVO_PLC = listEffect[k].listVO_PLC;
                                            newgroupDataBuf.listVS_PLC = listEffect[k].listVS_PLC;
                                            newgroupDataBuf.listLED_ArtnetDMX = listEffect[k].listLED_ArtnetDMX;
                                            newgroupDataBuf.listLED_PLC = listEffect[k].listLED_PLC;
                                        }
                                    }
                                    newgroupDataBuf.para1 = dataGridAllDataObserver.listMusicEffect[i].groupPara1;
                                    newgroupDataBuf.para2 = dataGridAllDataObserver.listMusicEffect[i].groupPara2;
                                    newgroupDataBuf.para3 = dataGridAllDataObserver.listMusicEffect[i].groupPara3;
                                    newgroupDataBuf.para4 = dataGridAllDataObserver.listMusicEffect[i].groupPara4;
                                    newgroupDataBuf.para5 = dataGridAllDataObserver.listMusicEffect[i].groupPara5;
                                    newgroupDataBuf.para6 = dataGridAllDataObserver.listMusicEffect[i].groupPara6;
                                    musicConfigBuf.listEffect[j].listEffectData.Add(newgroupDataBuf);
                                }
                            }

                            if (checkNewTimeID)
                            {
                                Data.Data_Config_Effect_With_Time newListEffectWithTimeBuf = new Data.Data_Config_Effect_With_Time();
                                newListEffectWithTimeBuf.musicTimeID = dataGridAllDataObserver.listMusicEffect[i].idTime;
                                for (int j = 0; j < dataGridAllDataObserver.listTime.Count; j++)
                                {
                                    if (newListEffectWithTimeBuf.musicTimeID == dataGridAllDataObserver.listTime[j].idTime)
                                    {
                                        newListEffectWithTimeBuf.musicTimeStart = double.Parse(dataGridAllDataObserver.listTime[j].timeStart);
                                        if (j < (dataGridAllDataObserver.listTime.Count - 1))
                                        {
                                            newListEffectWithTimeBuf.musicTimeStop = double.Parse(dataGridAllDataObserver.listTime[j + 1].timeStart);
                                            newListEffectWithTimeBuf.musicTimeLength = newListEffectWithTimeBuf.musicTimeStop - newListEffectWithTimeBuf.musicTimeStart;
                                        }
                                        else
                                        {
                                            newListEffectWithTimeBuf.musicTimeStop = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                                            newListEffectWithTimeBuf.musicTimeLength = newListEffectWithTimeBuf.musicTimeStop - newListEffectWithTimeBuf.musicTimeStart;
                                        }
                                    }
                                }

                                newListEffectWithTimeBuf.listEffectData = new List<Data.Data_Config_Effect>();
                                newListEffectWithTimeBuf.listEffectData.Clear();
                                Data.Data_Config_Effect newgroupDataBuf = new Data.Data_Config_Effect();
                                newgroupDataBuf.effectID = dataGridAllDataObserver.listMusicEffect[i].effectID;
                                newgroupDataBuf.effectName = dataGridAllDataObserver.listMusicEffect[i].effectName;
                                newgroupDataBuf.hardwareID = dataGridAllDataObserver.listMusicEffect[i].hardwareID;
                                newgroupDataBuf.groupID = dataGridAllDataObserver.listMusicEffect[i].groupID;
                                newgroupDataBuf.groupName = dataGridAllDataObserver.listMusicEffect[i].groupName;
                                newgroupDataBuf.groupType = dataGridAllDataObserver.listMusicEffect[i].groupType;
                                newgroupDataBuf.groupEffect = dataGridAllDataObserver.listMusicEffect[i].groupEffect;
                                for (int k = 0; k < listEffect.Count; k++)
                                {
                                    if ((newgroupDataBuf.hardwareID == listEffect[k].hardwareID) && (newgroupDataBuf.groupID == listEffect[k].groupID) && (newgroupDataBuf.effectID == listEffect[k].effectID))
                                    {
                                        newgroupDataBuf.listInverter = listEffect[k].listInverter;
                                        newgroupDataBuf.listVO_PLC = listEffect[k].listVO_PLC;
                                        newgroupDataBuf.listVS_PLC = listEffect[k].listVS_PLC;
                                        newgroupDataBuf.listLED_ArtnetDMX = listEffect[k].listLED_ArtnetDMX;
                                        newgroupDataBuf.listLED_PLC = listEffect[k].listLED_PLC;
                                    }
                                }

                                newgroupDataBuf.para1 = dataGridAllDataObserver.listMusicEffect[i].groupPara1;
                                newgroupDataBuf.para2 = dataGridAllDataObserver.listMusicEffect[i].groupPara2;
                                newgroupDataBuf.para3 = dataGridAllDataObserver.listMusicEffect[i].groupPara3;
                                newgroupDataBuf.para4 = dataGridAllDataObserver.listMusicEffect[i].groupPara4;
                                newgroupDataBuf.para5 = dataGridAllDataObserver.listMusicEffect[i].groupPara5;
                                newgroupDataBuf.para6 = dataGridAllDataObserver.listMusicEffect[i].groupPara6;
                                newListEffectWithTimeBuf.listEffectData.Add(newgroupDataBuf);

                                musicConfigBuf.listEffect.Add(newListEffectWithTimeBuf);
                            }
                        }
                        playMusicClick.Invoke(this, musicConfigBuf);
                    }
                }
            }
        }
        private void Pause_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            //pauseMusicClick.Invoke(this, 0);
        }
        private void Stop_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            StopPlay();
        }

        // For music time and UI
        private void SupportTimer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                {// Resync interval time for event
                    musicTime = mediaPlayer.Position.TotalMilliseconds;
                    readTimePre = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
                {// UI update
                    musicTime_Slider.Value = mediaPlayer.Position.TotalSeconds;
                    vLine.X = mediaPlayer.Position.TotalSeconds;
                    musicUI_WpfPlot.Render(lowQuality: true);
                    musicTime_Label.Content = mediaPlayer.Position.ToString(@"mm\:ss") + "/" + mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }
            }
            else
            {
                musicTime = 0;
            }
        }
        private void WpfPlot_Initialize(AudioFileReader audio)
        {
            double[] pcmValues;
            long MAX_INDICATE;
            long CountIndicate = 0;

            //Crosshair crosshair;
            int readed = 0;
            int SampleByte = audio.WaveFormat.BitsPerSample / 8;
            long totalSample = audio.Length / SampleByte;
            int NumberChanel = audio.WaveFormat.Channels;
            if (NumberChanel == 2) MAX_INDICATE = totalSample / 2;
            else MAX_INDICATE = totalSample;

            pcmValues = new double[MAX_INDICATE];

            float[] buffer = new float[4096];
            while ((readed = audio.Read(buffer, 0, 4096)) != 0)// readed number byte in buffer receiver
            {
                for (int i = 0; i < readed; i++)
                {
                    switch (NumberChanel)
                    {

                        case 2:
                            if ((i % 2) == 0)
                            {
                                pcmValues[CountIndicate] = buffer[i];
                                CountIndicate++;
                                if (CountIndicate >= MAX_INDICATE) CountIndicate = MAX_INDICATE - 1;
                            }
                            break;
                        default:
                            pcmValues[CountIndicate] = buffer[i];
                            CountIndicate++;
                            if (CountIndicate >= MAX_INDICATE) CountIndicate = MAX_INDICATE - 1;
                            break;
                    }

                }
            }

            musicUI_WpfPlot.Plot.Clear();

            musicUI_WpfPlot.Width = (int)audio.TotalTime.TotalSeconds * ZOOM_CONSTAN + 70;

            musicUI_WpfPlot.Plot.AddSignal(pcmValues, sampleRate: audio.WaveFormat.SampleRate);
            //musicUI_WpfPlot.Plot.AddVerticalLine(0, color: Color.Black, width: 2);
            //musicUI_WpfPlot.Plot.AddHorizontalLine(0, color: Color.Black, width: 2);
            //crosshair =musicUI_WpfPlot.Plot.AddCrosshair(0, 0);
            vLine = musicUI_WpfPlot.Plot.AddVerticalLine(0, color: Color.Yellow, width: 3);
            vLine.DragEnabled = true;
            // disable right-click-drag zoom
            musicUI_WpfPlot.Configuration.Zoom = false;
            musicUI_WpfPlot.Configuration.ScrollWheelZoom = false;
            musicUI_WpfPlot.Configuration.MiddleClickDragZoom = false;
            musicUI_WpfPlot.Configuration.MiddleClickAutoAxis = false;
            musicUI_WpfPlot.Configuration.Pan = false;
            musicUI_WpfPlot.Configuration.AllowDroppedFramesWhileDragging = true;
            musicUI_WpfPlot.Configuration.Quality = ScottPlot.Control.QualityMode.Low;
            musicUI_WpfPlot.Configuration.LockVerticalAxis = true;

            //double width = totalSample / 1000;
            //if (width > 10_000) width = 10_000;
            //if (width < 1000) width = 1000;
            //musicUI_WpfPlot.Width = width;

            //musicUI_WpfPlot.Plot.YLabel("Amplitude (%)");
            //musicUI_WpfPlot.Plot.XLabel("Time (seconds)");
            musicUI_WpfPlot.Plot.Style(ScottPlot.Style.Blue3);//ScottPlot.Style.Blue3

            //musicUI_WpfPlot.Plot.SetAxisLimits(0, MAX_INDICATE, -2, 2);
            musicUI_WpfPlot.Plot.SetAxisLimits(0, mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds, -1.5, 1.5);

            musicUI_WpfPlot.Render();
        }

        private void Refresh_GridTimeLine(int newMusicTime)
        {
            listTimeLine.Clear();
            // Creat a line each group
            for (int j = 0; j < listGroup.Count; j++)
            {
                Grid_TimeLine gridTimeLine = new Grid_TimeLine();
                gridTimeLine.headerName = listGroup[j].name;
                gridTimeLine.groupID = listGroup[j].groupID;
                gridTimeLine.hardwareID = listGroup[j].hardwareID;
                listTimeLine.Add(gridTimeLine);
            }
            // 
            for (int i = 0; i < dataGridAllDataObserver.listMusicEffect.Count; i++)
            {
                Grid_TimeLine.EffectTimeLine effectTimeLineBuf = new Grid_TimeLine.EffectTimeLine();
                effectTimeLineBuf.xPos = (int)(dataGridAllDataObserver.listMusicEffect[i].timeStart / 1000);
                effectTimeLineBuf.effectName = dataGridAllDataObserver.listMusicEffect[i].effectName;
                effectTimeLineBuf.timeLength = (int)(dataGridAllDataObserver.listMusicEffect[i].timeLength / 1000);
                effectTimeLineBuf.specialColor = ARRAY_EFFECT_COLORR[i % ARRAY_EFFECT_COLORR.Length];
                for (int j = 0; j < listTimeLine.Count; j++)
                {
                    if (dataGridAllDataObserver.listMusicEffect[i].groupID == listTimeLine[j].groupID &&
                        dataGridAllDataObserver.listMusicEffect[i].hardwareID == listTimeLine[j].hardwareID)
                    {
                        listTimeLine[j].listEffectTimeLine.Add(effectTimeLineBuf);
                    }
                }
            }
            Refresh_GridTimeLine_UI(listTimeLine, newMusicTime);
        }
        private void Refresh_GridTimeLine_UI(List<Grid_TimeLine> listTimeLine, int newMusicTime)
        {
            musicEffectTimeLine_Grid.Children.Clear();
            if (listTimeLine.Count > 0)
            {
                // Creat main stack panel
                StackPanel stackPanelAll = new StackPanel();
                stackPanelAll.Height = listTimeLine.Count * 24;
                stackPanelAll.Background = Brushes.DarkGray;
                stackPanelAll.Orientation = Orientation.Vertical;
                for (int i = 0; i < listTimeLine.Count; i++)
                {
                    // Creat one by one row in timeline
                    Grid oneLineGrid = new Grid();
                    oneLineGrid.Height = 20;
                    oneLineGrid.Width = newMusicTime * ZOOM_CONSTAN + 200;
                    oneLineGrid.Background = Brushes.Black;
                    oneLineGrid.Margin = new Thickness(0, 2, 2, 2);
                    oneLineGrid.HorizontalAlignment = HorizontalAlignment.Left;

                    {// Creat header of row
                        TextBlock headerTextBuf = new TextBlock();
                        headerTextBuf.Text = listTimeLine[i].headerName;
                        headerTextBuf.Foreground = Brushes.Wheat;
                        headerTextBuf.Width = 195;
                        headerTextBuf.Margin = new Thickness(5, 0, 0, 0);
                        headerTextBuf.HorizontalAlignment = HorizontalAlignment.Left;
                        oneLineGrid.Children.Add(headerTextBuf);
                    }

                    for (int k = 0; k < listTimeLine[i].listEffectTimeLine.Count; k++)
                    {
                        {// Creat one by one effect in a row
                            TextBlock effectTextBuf = new TextBlock();
                            effectTextBuf.Text = listTimeLine[i].listEffectTimeLine[k].effectName;
                            effectTextBuf.Background = listTimeLine[i].listEffectTimeLine[k].specialColor;
                            effectTextBuf.Foreground = Brushes.Wheat;
                            effectTextBuf.Margin = new Thickness(200 + listTimeLine[i].listEffectTimeLine[k].xPos * ZOOM_CONSTAN, 1, 0, 1);
                            effectTextBuf.HorizontalAlignment = HorizontalAlignment.Stretch;
                            effectTextBuf.VerticalAlignment = VerticalAlignment.Stretch;
                            oneLineGrid.Children.Add(effectTextBuf);
                        }
                    }
                    // Add oneLineGrid to main stack panel
                    stackPanelAll.Children.Add(oneLineGrid);
                }
                musicEffectTimeLine_Grid.Children.Add(stackPanelAll);
            }
        }

        private void Update_MusicTime_Thread()
        {
            while (updateMusicTimeThreadRunning)
            {
                try
                {
                    if (mediaPlayerState)
                    {
                        realTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        playMusicTimeIntervalUpdate.Invoke(this, musicTime + realTime - readTimePre);
                        Thread.Sleep(5);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void MediaEnded_EventHandle(object sender, EventArgs e)
        {
            try
            {
                StopPlay();
                MessageBox.Show("Bản nhạc kết thúc\nĐã dừng tất cả thiết bị");
            }
            catch (Exception)
            {

            }
        }
        private void MediaFailed_EventHandle(object sender, EventArgs e)
        {
            try
            {
                StopPlay();
                MessageBox.Show("Trình phát nhạc xảy ra lỗi\nĐã dừng tất cả thiết bị");
            }
            catch (Exception)
            {

            }
        }

        // Public methods
        public void UpdateListMusicConfig(List<Data.Data_Config_Music> newListMusicConfig)
        {
            listMusic.Clear();
            for (int i = 0; i < newListMusicConfig.Count; i++)
            {
                listMusic.Add(newListMusicConfig[i]);
            }
            MusicSelection_ComboBoxChanged_EventHandle(null, null);
        }
        public void UpdateListGroupConfig(List<Data.Data_Config_Hardware_Group> newListGroup)
        {
            listGroup.Clear();
            listGroupName.Clear();
            for (int i = 0; i < newListGroup.Count; i++)
            {
                listGroup.Add(newListGroup[i]);
                listGroupName.Add(newListGroup[i].name);
            }
            MusicSelection_ComboBoxChanged_EventHandle(null, null);
        }
        public void UpdateListEffectConfig(List<Data.Data_Config_Effect> newListEffectConfig)
        {
            listEffect.Clear();
            listEffectName.Clear();
            for (int i = 0; i < newListEffectConfig.Count; i++)
            {
                listEffect.Add(newListEffectConfig[i]);
                listEffectName.Add(newListEffectConfig[i].effectName);
            }
            effectName_ComboBox.ItemsSource = listEffectName;
            CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
        }
        public void StopPlay()
        {
            if (mediaPlayerState)
            {
                mediaPlayerState = false;
                mediaPlayer.Stop();
                stopMusicClick.Invoke(this, 0);
            }
        }

        public void Update_MusicTime_ThreadStart()
        {
            updateMusicTimeThreadRunning = true;
            updateMusicTimeThread.Start();
        }
        public void Update_MusicTime_ThreadStop()
        {
            updateMusicTimeThreadRunning = false;
        }

        public void ApplicationExit()
        {
            Update_MusicTime_ThreadStop();
            StopPlay();
        }

    }
}