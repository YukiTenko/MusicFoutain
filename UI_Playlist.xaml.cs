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
using System.Timers;
using System.Threading;
using ScottPlot.Plottable;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Color = System.Drawing.Color;
using System.Windows.Threading;
using System.IO;

namespace MusicFountain.UI.Playlist
{
    /// <summary>
    /// Interaction logic for UI_Playlist.xaml
    /// </summary>
    public partial class UI_Playlist : Page
    {
        // Support class
        public class DataGrid_Music
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class DataGrid_MusicEffect
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
        public class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_Music> listMusic;
            public ObservableCollection<DataGrid_MusicEffect> listMusicEffect;
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
        private SolidColorBrush[] ARRAY_EFFECT_COLORR = { Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkCyan, Brushes.DarkGreen, Brushes.DarkSlateGray, Brushes.DarkMagenta };

        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
        // For backend data
        private List<string> listMusicName;
        private List<string> listGroupName;
        List<Data.Data_Config_Music> listMusic;
        private List<Data.Data_Config_Hardware_Group> listGroup;
        Data.Data_Config_Playlist_Mode playlistMode;
        // For play media
        private MediaPlayer mediaPlayer;
        private bool mediaPlayerState;
        private VLine vLine;
        private List<Grid_TimeLine> listTimeLine;
        // For music time
        private DispatcherTimer supportTimer;
        private Thread updateMusicTimeThread;
        private bool updateMusicTimeThreadRunning;
        private double musicTime;
        private double realTime;
        private double readTimePre;

        // Events
        public event EventHandler<Data.Data_Config_Playlist_Mode> playlistModeSaveClick;
        public event EventHandler<Data.Data_Config_Music> playMusicClick;
        //public event EventHandler<int> pauseMusicClick;
        public event EventHandler<int> stopMusicClick;
        public event EventHandler<double> playMusicTimeIntervalUpdate;

        public UI_Playlist()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listMusic = new ObservableCollection<DataGrid_Music>();
                    dataGridAllDataObserver.listMusicEffect = new ObservableCollection<DataGrid_MusicEffect>();
                }
                {// For backend process
                    listMusicName = new List<string>();
                    listGroupName = new List<string>();
                    listMusic = new List<Data.Data_Config_Music>();
                    listGroup = new List<Data.Data_Config_Hardware_Group>();
                    playlistMode = new Data.Data_Config_Playlist_Mode();

                    string[] listMusicFilesBuf = Directory.GetFiles("../Media");
                    listMusicName.Clear();
                    foreach (string musicFileBuf in listMusicFilesBuf)
                    {
                        listMusicName.Add(musicFileBuf);
                    }
                    listTimeLine = new List<Grid_TimeLine>();

                    mediaPlayerState = false;
                    playPause_Button.Content = "Chạy";
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
                    listMusicFile_DataGrid.ItemsSource = dataGridAllDataObserver.listMusic;
                    CollectionViewSource.GetDefaultView(listMusicFile_DataGrid.ItemsSource).Refresh();

                    listMusicEffect_DataGrid.ItemsSource = dataGridAllDataObserver.listMusicEffect;
                    CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                    dataGridAllDataObserver.listMusic.Clear();
                    for (int i = 0; i < listMusicName.Count; i++)
                    {
                        DataGrid_Music dataGridMusicBuf = new DataGrid_Music();
                        dataGridMusicBuf.id = i + 1;
                        dataGridMusicBuf.name = listMusicName[i];
                        dataGridAllDataObserver.listMusic.Add(dataGridMusicBuf);
                    }
                    CollectionViewSource.GetDefaultView(listMusicFile_DataGrid.ItemsSource).Refresh();
                }
            }
        }

        // Private methods
        private void PlayPrevious_Click_EventHandle(object sender, RoutedEventArgs e)
        {
            StopPlay();
            try
            {
                // Get new select index of music
                int selectIndexBuf = listMusicFile_DataGrid.SelectedIndex;
                selectIndexBuf--;
                if (selectIndexBuf < 0)
                {
                    selectIndexBuf = listMusicName.Count - 1;
                }
                listMusicFile_DataGrid.SelectedIndex = selectIndexBuf;
                // Play music
                bool checkMusicBuf = false;
                for (int k = 0; k < listMusic.Count; k++)
                {
                    if (listMusic[k].musicPath == listMusicName[listMusicFile_DataGrid.SelectedIndex])
                    {
                        mediaPlayer.Open(new System.Uri(System.IO.Path.GetFullPath(listMusicName[listMusicFile_DataGrid.SelectedIndex])));

                        ListMusicFile_SelectionChanged_EventHandle(null, null); // Refresh UI
                        mediaPlayer.Play();

                        playPause_Button.Content = "Dừng";
                        mediaPlayerState = true;

                        playMusicClick.Invoke(this, listMusic[k]);

                        checkMusicBuf = true;
                    }
                }
                if (checkMusicBuf == false)
                {
                    MessageBox.Show("Bản nhạc này chưa được tạo hiệu ứng\nVui lòng tạo hiệu ứng trước khi phát nhạc");
                }
            }
            catch (Exception)
            {

            }
        }
        private void PlayPause_Click_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mediaPlayerState)
                {
                    StopPlay();
                }
                else
                {
                    if (listMusicFile_DataGrid.SelectedIndex < listMusicName.Count)
                    {// Play music
                        ListMusicFile_SelectionChanged_EventHandle(null, null); // Refresh UI
                        bool checkMusicBuf = false;
                        for (int k = 0; k < listMusic.Count; k++)
                        {
                            if (listMusic[k].musicPath == listMusicName[listMusicFile_DataGrid.SelectedIndex])
                            {
                                mediaPlayer.Open(new System.Uri(System.IO.Path.GetFullPath(listMusicName[listMusicFile_DataGrid.SelectedIndex])));

                                ListMusicFile_SelectionChanged_EventHandle(null, null); // Refresh UI
                                mediaPlayer.Play();

                                playPause_Button.Content = "Dừng";
                                mediaPlayerState = true;

                                playMusicClick.Invoke(this, listMusic[k]);

                                checkMusicBuf = true;
                            }
                        }
                        if (checkMusicBuf == false)
                        {
                            MessageBox.Show("Bản nhạc này chưa được tạo hiệu ứng\nVui lòng tạo hiệu ứng trước khi phát nhạc");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi chọn bản nhạc muốn phát");
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void PlayNext_Click_EventHandle(object sender, RoutedEventArgs e)
        {
            StopPlay();
            try
            {
                // Get new select index of music
                int selectIndexBuf = listMusicFile_DataGrid.SelectedIndex;
                selectIndexBuf++;
                if (selectIndexBuf >= listMusicName.Count)
                {
                    selectIndexBuf = 0;
                }
                listMusicFile_DataGrid.SelectedIndex = selectIndexBuf;
                ListMusicFile_SelectionChanged_EventHandle(null, null); // Refresh UI
                // Play music
                bool checkMusicBuf = false;
                for (int k = 0; k < listMusic.Count; k++)
                {
                    if (listMusic[k].musicPath == listMusicName[listMusicFile_DataGrid.SelectedIndex])
                    {
                        mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(listMusicName[listMusicFile_DataGrid.SelectedIndex])));

                        ListMusicFile_SelectionChanged_EventHandle(null, null); // Refresh UI
                        mediaPlayer.Play();

                        playPause_Button.Content = "Dừng";
                        mediaPlayerState = true;

                        playMusicClick.Invoke(this, listMusic[k]);

                        checkMusicBuf = true;
                    }
                }
                if (checkMusicBuf == false)
                {
                    MessageBox.Show("Bản nhạc này chưa được tạo hiệu ứng\nVui lòng tạo hiệu ứng trước khi phát nhạc");
                }
            }
            catch (Exception)
            {

            }
        }
        private void PlayListModeSave_Click_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                {// Get new mode info
                    playlistMode.isRepeatOne = repeatOne_RadioButton.IsChecked.Value;

                    playlistMode.isAutoStart = timeStart_CheckBox.IsChecked.Value;
                    if (playlistMode.isAutoStart)
                    {
                        playlistMode.hourAutoStart = int.Parse(hourStart_TextBox.Text);
                        playlistMode.minAutoStart = int.Parse(minStart_TextBox.Text);
                    }

                    playlistMode.isAutoStop = timeStop_CheckBox.IsChecked.Value;
                    if (playlistMode.isAutoStop)
                    {
                        playlistMode.hourAutoStop = int.Parse(hourStop_TextBox.Text);
                        playlistMode.minAutoStop = int.Parse(minStop_TextBox.Text);
                    }
                }

                {// Check mode info and send event
                    bool checkTimeBuf = true;
                    // Check hour start
                    if (playlistMode.isAutoStart && ((playlistMode.hourAutoStart > 23) || (playlistMode.hourAutoStart < 0)))
                    {
                        checkTimeBuf = false;
                    }
                    // Check min start
                    if (playlistMode.isAutoStart && ((playlistMode.minAutoStart > 59) || (playlistMode.minAutoStart < 0)))
                    {
                        checkTimeBuf = false;
                    }
                    // Check hour stop
                    if (playlistMode.isAutoStop && ((playlistMode.hourAutoStop > 23) || (playlistMode.hourAutoStop < 0)))
                    {
                        checkTimeBuf = false;
                    }
                    // Check min start
                    if (playlistMode.isAutoStop && ((playlistMode.minAutoStop > 59) || (playlistMode.minAutoStop < 0)))
                    {
                        checkTimeBuf = false;
                    }
                    // Compare time start and stop
                    if (playlistMode.isAutoStart && playlistMode.isAutoStop && ((playlistMode.hourAutoStart > playlistMode.hourAutoStop) || ((playlistMode.hourAutoStart == playlistMode.hourAutoStop) && (playlistMode.minAutoStart >= playlistMode.minAutoStop))))
                    {
                        checkTimeBuf = false;
                    }

                    if (checkTimeBuf)
                    {
                        playlistModeSaveClick.Invoke(this, playlistMode);
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chỉ kiểm tra lại các mốc thời gian đã nhập");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Vui lòng chỉ kiểm tra lại các mốc thời gian đã nhập");
            }
        }
        private void ListMusicFile_SelectionChanged_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listMusicFile_DataGrid.SelectedIndex < listMusicName.Count)
                {
                    dataGridAllDataObserver.listMusicEffect.Clear();
                    for (int k = 0; k < listMusic.Count; k++)
                    {
                        if (listMusic[k].musicPath == listMusicName[listMusicFile_DataGrid.SelectedIndex])
                        {
                            int idCountBuf = 1;
                            // Update listMusicEffect_DataGrid
                            for (int i = 0; i < listMusic[k].listEffect.Count; i++)
                            {
                                for (int j = 0; j < listMusic[k].listEffect[i].listEffectData.Count; j++)
                                {
                                    DataGrid_MusicEffect dataGrid_MusicEffectBuf = new DataGrid_MusicEffect();
                                    dataGrid_MusicEffectBuf.id = idCountBuf;
                                    idCountBuf++;
                                    dataGrid_MusicEffectBuf.idTime = listMusic[k].listEffect[i].musicTimeID;
                                    dataGrid_MusicEffectBuf.timeLength = listMusic[k].listEffect[i].musicTimeLength;
                                    dataGrid_MusicEffectBuf.timeStart = listMusic[k].listEffect[i].musicTimeStart;
                                    dataGrid_MusicEffectBuf.timeStop = listMusic[k].listEffect[i].musicTimeStop;

                                    dataGrid_MusicEffectBuf.effectID = listMusic[k].listEffect[i].listEffectData[j].effectID;
                                    dataGrid_MusicEffectBuf.effectName = listMusic[k].listEffect[i].listEffectData[j].effectName;
                                    dataGrid_MusicEffectBuf.hardwareID = listMusic[k].listEffect[i].listEffectData[j].hardwareID;
                                    dataGrid_MusicEffectBuf.groupID = listMusic[k].listEffect[i].listEffectData[j].groupID;
                                    dataGrid_MusicEffectBuf.groupName = listMusic[k].listEffect[i].listEffectData[j].groupName;
                                    dataGrid_MusicEffectBuf.groupType = listMusic[k].listEffect[i].listEffectData[j].groupType;
                                    dataGrid_MusicEffectBuf.groupEffect = listMusic[k].listEffect[i].listEffectData[j].groupEffect;

                                    dataGrid_MusicEffectBuf.groupPara1 = listMusic[k].listEffect[i].listEffectData[j].para1;
                                    dataGrid_MusicEffectBuf.groupPara2 = listMusic[k].listEffect[i].listEffectData[j].para2;
                                    dataGrid_MusicEffectBuf.groupPara3 = listMusic[k].listEffect[i].listEffectData[j].para3;
                                    dataGrid_MusicEffectBuf.groupPara4 = listMusic[k].listEffect[i].listEffectData[j].para4;
                                    dataGrid_MusicEffectBuf.groupPara5 = listMusic[k].listEffect[i].listEffectData[j].para5;
                                    dataGrid_MusicEffectBuf.groupPara6 = listMusic[k].listEffect[i].listEffectData[j].para6;
                                    dataGridAllDataObserver.listMusicEffect.Add(dataGrid_MusicEffectBuf);
                                }
                            }
                        }
                    }

                    AudioFileReader stream;
                    stream = new AudioFileReader(listMusicName[listMusicFile_DataGrid.SelectedIndex]);
                    WpfPlot_Initialize(stream);
                    Refresh_GridTimeLine((int)stream.TotalTime.TotalSeconds);
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi chọn bản nhạc muốn phát");
                }
            }
            catch (Exception)
            {

            }
        }
        
        // For music time and UI
        private void SupportTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                timeNow_Label.Content = DateTime.Now.ToString("HH:mm:ss");
                if (mediaPlayer.Source != null)
                {
                    {// Resync interval time for event
                        musicTime = mediaPlayer.Position.TotalMilliseconds;
                        readTimePre = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    }
                    {// UI update
                        vLine.X = mediaPlayer.Position.TotalSeconds;
                        //musicUI_WpfPlot.Plot.SetAxisLimits(mediaPlayer.Position.TotalSeconds - 10, mediaPlayer.Position.TotalSeconds + 10, -1.5, 1.5);
                        musicUI_WpfPlot.Render(lowQuality: true);
                        if (mediaPlayerState)
                        {
                            musicNameAndTime_Label.Content = "Đang phát: " + listMusicName[listMusicFile_DataGrid.SelectedIndex] + ", thời gian: " + mediaPlayer.Position.ToString(@"mm\:ss") + "/" + mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                        }
                        else
                        {
                            musicNameAndTime_Label.Content = "Đang dừng";
                        }
                    }

                }
                else
                {
                    musicTime = 0;
                }
                {// Check time start and stop
                    int hourNowBuf = DateTime.Now.Hour;
                    int minNowBuf = DateTime.Now.Minute;
                    bool checkTimeBuf = true; // check time is in time start and stop
                    {// Compare time
                        if ((hourNowBuf < playlistMode.hourAutoStart) || ((hourNowBuf == playlistMode.hourAutoStart) && (minNowBuf < playlistMode.minAutoStart)))
                        {
                            checkTimeBuf = false;
                        }
                        if ((hourNowBuf > playlistMode.hourAutoStop) || ((hourNowBuf == playlistMode.hourAutoStop) && (minNowBuf > playlistMode.minAutoStop)))
                        {
                            checkTimeBuf = false;
                        }
                    }
                    if (playlistMode.isAutoStart && checkTimeBuf)
                    {
                        if (mediaPlayerState == false)
                        {
                            PlayPause_Click_EventHandle(null, null);
                        }
                    }
                    if (playlistMode.isAutoStop && (!checkTimeBuf))
                    {
                        if (mediaPlayerState == true)
                        {
                            StopPlay();
                        }
                    }
                }
            }
            catch (Exception)
            {

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
            musicUI_WpfPlot.Plot.SetAxisLimits(0, audio.TotalTime.TotalSeconds, -1.5, 1.5);

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
                if (playlistMode.isRepeatOne)
                {
                    PlayPause_Click_EventHandle(null, null);
                }
                else
                {
                    PlayNext_Click_EventHandle(null, null);
                }
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
            listMusic = newListMusicConfig;
            dataGridAllDataObserver.listMusicEffect.Clear();
            int idCountBuf = 1;
            if (listMusic.Count > 0)
            {
                // Update listMusicEffect_DataGrid
                for (int i = 0; i < listMusic[0].listEffect.Count; i++)
                {
                    for (int j = 0; j < listMusic[0].listEffect[i].listEffectData.Count; j++)
                    {
                        DataGrid_MusicEffect dataGrid_MusicEffectBuf = new DataGrid_MusicEffect();
                        dataGrid_MusicEffectBuf.id = idCountBuf;
                        idCountBuf++;
                        dataGrid_MusicEffectBuf.idTime = listMusic[0].listEffect[i].musicTimeID;
                        dataGrid_MusicEffectBuf.hardwareID = listMusic[0].listEffect[i].listEffectData[j].hardwareID;
                        dataGrid_MusicEffectBuf.groupID = listMusic[0].listEffect[i].listEffectData[j].groupID;
                        dataGrid_MusicEffectBuf.groupName = listMusic[0].listEffect[i].listEffectData[j].groupName;
                        dataGrid_MusicEffectBuf.groupType = listMusic[0].listEffect[i].listEffectData[j].groupType;
                        dataGrid_MusicEffectBuf.groupEffect = listMusic[0].listEffect[i].listEffectData[j].groupEffect;
                        dataGrid_MusicEffectBuf.groupPara1 = listMusic[0].listEffect[i].listEffectData[j].para1;
                        dataGrid_MusicEffectBuf.groupPara2 = listMusic[0].listEffect[i].listEffectData[j].para2;
                        dataGrid_MusicEffectBuf.groupPara3 = listMusic[0].listEffect[i].listEffectData[j].para3;
                        dataGrid_MusicEffectBuf.groupPara4 = listMusic[0].listEffect[i].listEffectData[j].para4;
                        dataGrid_MusicEffectBuf.groupPara5 = listMusic[0].listEffect[i].listEffectData[j].para5;
                        dataGrid_MusicEffectBuf.groupPara6 = listMusic[0].listEffect[i].listEffectData[j].para6;
                        dataGridAllDataObserver.listMusicEffect.Add(dataGrid_MusicEffectBuf);
                    }
                }
                CollectionViewSource.GetDefaultView(listMusicEffect_DataGrid.ItemsSource).Refresh();
                // Change selection index in listMusicFile_DataGrid
                for (int i = 0; i < listMusicName.Count; i++)
                {
                    if (listMusic[0].musicPath == listMusicName[i])
                    {
                        listMusicFile_DataGrid.SelectedIndex = i;
                    }
                }
            }
        }
        public void UpdatePlaylistMode(Data.Data_Config_Playlist_Mode newPlaylistMode)
        {
            playlistMode = newPlaylistMode;
            if (playlistMode.isRepeatOne)
            {
                repeatOne_RadioButton.IsChecked = true;
                repeatAll_RadioButton.IsChecked = false;
            }
            else
            {
                repeatOne_RadioButton.IsChecked = false;
                repeatAll_RadioButton.IsChecked = true;
            }
            timeStart_CheckBox.IsChecked = playlistMode.isAutoStart;
            hourStart_TextBox.Text = playlistMode.hourAutoStart.ToString("00");
            minStart_TextBox.Text = playlistMode.minAutoStart.ToString("00");
            timeStop_CheckBox.IsChecked = playlistMode.isAutoStop;
            hourStop_TextBox.Text = playlistMode.hourAutoStop.ToString("00");
            minStop_TextBox.Text = playlistMode.minAutoStop.ToString("00");
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
        }

        public void StopPlay()
        {
            try
            {
                if (mediaPlayerState)
                {
                    mediaPlayerState = false;
                    playPause_Button.Content = "Chạy";
                    mediaPlayer.Stop();
                    stopMusicClick.Invoke(this, 0);
                }
            }
            catch
            {

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
