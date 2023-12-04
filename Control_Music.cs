using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MusicFountain
{
    public partial class Control_Music
    {
        // Variables
        private Data.Data_Config_Music musicConfig;
        private double musicTime;
        private Thread musicThread;
        private bool running;

        // Events
        public event EventHandler<Data.Data_Config_Hardware_Group> groupChange;

        public Control_Music(Data.Data_Config_Music newMusicConfig)
        {
            musicConfig = newMusicConfig;

            musicThread = new Thread(Control_Music_Thread);
        }

        // Public methods
        public void Thread_Start()
        {
            running = true;
            musicThread.Start();
        }
        public void Thread_Stop()
        {
            running = false;
        }

        public void Set_NewMusicConfig(Data.Data_Config_Music newMusicConfig)
        {
            musicConfig = newMusicConfig;
        }
        public void Set_NewMusicTime(double newMusicTime)
        {
            musicTime = newMusicTime;
        }

        // Private methods
        private void Control_Music_Thread()
        {
            while (running)
            {
                try
                {
                    if (musicConfig != null)
                    {
                        for (int i = 0; i < musicConfig.listEffect.Count; i++)
                        {
                            if ((musicTime > musicConfig.listEffect[i].musicTimeStart) && (musicTime < musicConfig.listEffect[i].musicTimeStop))
                            {
                                for (int j = 0; j < musicConfig.listEffect[i].listEffectData.Count; j++)
                                {
                                    Data.Data_Config_Hardware_Group newGroupConfig = new Data.Data_Config_Hardware_Group();
                                    newGroupConfig.hardwareID = musicConfig.listEffect[i].listEffectData[j].hardwareID;
                                    newGroupConfig.groupID = musicConfig.listEffect[i].listEffectData[j].groupID;
                                    newGroupConfig.name = musicConfig.listEffect[i].listEffectData[j].groupName;
                                    newGroupConfig.type = musicConfig.listEffect[i].listEffectData[j].groupType;
                                    newGroupConfig.effectRunning = musicConfig.listEffect[i].listEffectData[j].groupEffect;
                                    newGroupConfig.listLED_ArtnetDMX = musicConfig.listEffect[i].listEffectData[j].listLED_ArtnetDMX;
                                    newGroupConfig.listLED_PLC = musicConfig.listEffect[i].listEffectData[j].listLED_PLC;
                                    newGroupConfig.listInverter = musicConfig.listEffect[i].listEffectData[j].listInverter;
                                    newGroupConfig.listVO_PLC = musicConfig.listEffect[i].listEffectData[j].listVO_PLC;
                                    newGroupConfig.listVS_PLC = musicConfig.listEffect[i].listEffectData[j].listVS_PLC;
                                    switch (newGroupConfig.type)
                                    {
                                        case "Valve On/Off - PLC":
                                            {
                                                try
                                                {
                                                    newGroupConfig.timeHoldOff = int.Parse(musicConfig.listEffect[i].listEffectData[j].para1);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOff = 10000;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOn = int.Parse(musicConfig.listEffect[i].listEffectData[j].para2);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOn = 1000;
                                                }
                                            }
                                            break;
                                        case "Valve Stepper - PLC":
                                            {
                                                try
                                                {
                                                    newGroupConfig.speed = int.Parse(musicConfig.listEffect[i].listEffectData[j].para1);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.speed = 40;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOff = int.Parse(musicConfig.listEffect[i].listEffectData[j].para2);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOff = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOn = int.Parse(musicConfig.listEffect[i].listEffectData[j].para3);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOn = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.startPosition = int.Parse(musicConfig.listEffect[i].listEffectData[j].para4);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.startPosition = 70;
                                                }
                                                try
                                                {
                                                    newGroupConfig.endPosition = int.Parse(musicConfig.listEffect[i].listEffectData[j].para5);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.startPosition = 110;
                                                }
                                            }
                                            break;
                                        case "Pump Analog - PLC":
                                        case "Pump Analog Dual - PLC":
                                            {
                                                try
                                                {
                                                    newGroupConfig.freqMax = int.Parse(musicConfig.listEffect[i].listEffectData[j].para1);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.freqMax = 30;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeGoOn = int.Parse(musicConfig.listEffect[i].listEffectData[j].para2);
                                                    newGroupConfig.cycle = int.Parse(musicConfig.listEffect[i].listEffectData[j].para2);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeGoOn = 10000;
                                                    newGroupConfig.cycle = 10000;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOn = int.Parse(musicConfig.listEffect[i].listEffectData[j].para3);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOn = 10000;
                                                }
                                                try
                                                {
                                                    newGroupConfig.freqMin = int.Parse(musicConfig.listEffect[i].listEffectData[j].para5);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.freqMin = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeGoOff = int.Parse(musicConfig.listEffect[i].listEffectData[j].para5);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeGoOff = 10000;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOff = int.Parse(musicConfig.listEffect[i].listEffectData[j].para5);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOff = 10000;
                                                }
                                            }
                                            break;
                                        case "LED 485 - PLC":
                                        case "LED DMX - Artnet":
                                            {
                                                try
                                                {
                                                    newGroupConfig.timeHoldOff = int.Parse(musicConfig.listEffect[i].listEffectData[j].para1);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOff = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.timeHoldOn = int.Parse(musicConfig.listEffect[i].listEffectData[j].para2);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.timeHoldOn = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.red = int.Parse(musicConfig.listEffect[i].listEffectData[j].para3);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.red = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.green = int.Parse(musicConfig.listEffect[i].listEffectData[j].para4);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.green = 0;
                                                }
                                                try
                                                {
                                                    newGroupConfig.blue = int.Parse(musicConfig.listEffect[i].listEffectData[j].para5);
                                                }
                                                catch (Exception)
                                                {
                                                    newGroupConfig.blue = 0;
                                                }
                                            }
                                            break;
                                        case "Pump Digital - PLC":
                                        case "System Power":
                                        default:
                                            {

                                            }
                                            break;
                                    }
                                    groupChange.Invoke(this, newGroupConfig);
                                }

                            }
                        }
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Chỉnh sửa cấu hình phần cứng hoặc bật tắt liên tiếp có thể gây ra lỗi\nVui lòng kiểm tra lại thông tin cấu hình các bản nhạc hoặc dừng hành động bật/tắt liên tiếp");
                }
            }
            //MessageBox.Show("Control_Music_Thread of " + musicConfig.musicPath +  " is stopped !!!");
        }
    }
}
