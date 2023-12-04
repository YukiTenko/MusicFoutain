using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicFountain
{
    public partial class Control_Group
    {
        // Constant
        private int EFFECT_INTERVAL_RESEND = 3000;

        // Variables
        private Data.Data_Config_Hardware hardwareConfig;
        private Data.Data_Config_Hardware_Group groupConfig;

        private bool effectNewByTestMode;
        private string effectRunning;
        private int effectSpecialCount;
        private double effectSendByMusicTime;

        private double musicTime;
        private Thread groupThread;
        private bool running;

        // Events
        public event EventHandler<Data.Data_Config_Hardware_Group> changeVOPLC_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changeVSPLC_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changePumpDigital_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changePumpAnalog_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changeLEDPLC_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changeLEDArtnet_Data;
        public event EventHandler<Data.Data_Config_Hardware_Group> changeSystemPower_Data;

        public Control_Group(Data.Data_Config_Hardware newHardwareConfig, Data.Data_Config_Hardware_Group newGroupConfig)
        {
            hardwareConfig = newHardwareConfig;
            groupConfig = newGroupConfig;

            effectNewByTestMode = false;
            effectRunning = "";
            effectSpecialCount = 0;
            effectSendByMusicTime = 0;

            musicTime = 0;
            groupThread = new Thread(Control_Group_Thread);
        }

        // Public methods
        public void Thread_Start()
        {
            running = true;
            groupThread.Start();
        }
        public void Thread_Stop()
        {
            running = false;
        }

        public int Get_HardwareID()
        {
            return groupConfig.hardwareID;
        }
        public int Get_GroupID()
        {
            return groupConfig.groupID;
        }

        public void SetEffectToSpecialStop()
        {
            if (groupConfig != null)
            {
                groupConfig.effectRunning = "Tắt toàn bộ";
            }
        }

        public void Set_NewGroupConfig( Data.Data_Config_Hardware_Group newGroupConfig)
        {
            groupConfig = newGroupConfig;
            if (groupConfig.testMode)
            {
                effectNewByTestMode = true;
            }
        }
        public void Set_NewMusicTime(double newMusicTime)
        {
            musicTime = newMusicTime;
        }

        // Private methods
        private void Control_Group_Thread()
        {
            while (running)
            {
                try
                {
                    if ((effectRunning != groupConfig.effectRunning) || (effectNewByTestMode))
                    {// Reset all variables when change effect
                        effectNewByTestMode = false;
                        effectRunning = groupConfig.effectRunning;
                        effectSpecialCount = 0;
                        if (groupConfig.testMode)
                        {
                            musicTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        }
                        else
                        {
                            musicTime = 0;
                        }
                        effectSendByMusicTime = -9999;
                    }
                    if (groupConfig.testMode)
                    {// Get time for testMode
                        musicTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    }
                    switch (groupConfig.type)
                    {
                        case "Valve Stepper - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// OFF all valve
                                                    groupConfig.listVS_PLC[i].enable = false;
                                                    groupConfig.listVS_PLC[i].setSpeed = 0;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Về gốc tọa độ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Reset all valve
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    groupConfig.listVS_PLC[i].setAngle = -1;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Không vẫy":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Reset all valve
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Vẫy đồng thời":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn + groupConfig.timeHoldOff); // 2 times move + time wait at max and min
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn))
                                                {// in time go to end
                                                    if (effectSpecialCount == 1)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 2;
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount == 2)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 1;
                                                    }
                                                }
                                            }
                                            if ((musicTime - effectSendByMusicTime) >= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + groupConfig.timeHoldOff))
                                            {// Next effect step
                                            }
                                        }
                                        break;
                                    case "Vẫy đồng thời - xen kẽ 1 van":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn + groupConfig.timeHoldOff); // 2 times move + time wait at max and min
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    if ((i % 2) == 0)
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                    }
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn))
                                                {// in time go to end
                                                    if (effectSpecialCount == 1)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if ((i % 2) == 0)
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 2;
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount == 2)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if ((i % 2) == 0)
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Vẫy đồng thời - xen kẽ 2 van":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn + groupConfig.timeHoldOff); // 2 times move + time wait at max and min
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    if ((i % 4) < 2)
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                    }
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn))
                                                {// in time go to end
                                                    if (effectSpecialCount == 1)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if ((i % 4) < 2)
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 2;
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount == 2)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if ((i % 4) < 2)
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Hai nửa đối xứng":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn + groupConfig.timeHoldOff); // 2 times move + time wait at max and min
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    if (i < (groupConfig.listVS_PLC.Count/2))
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                    }
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + groupConfig.timeHoldOn))
                                                {// in time go to end
                                                    if (effectSpecialCount == 1)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (i < (groupConfig.listVS_PLC.Count / 2))
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 2;
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount == 2)
                                                    {// Onlye send one time
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (i < (groupConfig.listVS_PLC.Count / 2))
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                        }
                                                        changeVSPLC_Data.Invoke(this, groupConfig);
                                                        effectSpecialCount = 1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Vẫy lần lượt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn + groupConfig.timeHoldOff) * groupConfig.listVS_PLC.Count); // 2 times move + (time wait max and min) * number of valve
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn * groupConfig.listVS_PLC.Count)))
                                                {// in time go to end
                                                    if (effectSpecialCount >= 1)
                                                    {// Only send when a valve change
                                                        bool checkNeedSendBuf = false;
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (myTimeBuf > (i * groupConfig.timeHoldOn))
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                                if ((i + 2) > effectSpecialCount)
                                                                {// Only send when valve change
                                                                    effectSpecialCount = i + 2;
                                                                    checkNeedSendBuf = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                            }
                                                        }
                                                        if (checkNeedSendBuf)
                                                        {// Only send if need
                                                            changeVSPLC_Data.Invoke(this, groupConfig);
                                                            effectSpecialCount = 2;
                                                        }
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount < (groupConfig.listVS_PLC.Count * 2))
                                                    {// Reload effectSpecialCount
                                                        effectSpecialCount = groupConfig.listVS_PLC.Count * 2;
                                                    }
                                                    if (effectSpecialCount >= groupConfig.listVS_PLC.Count * 2)
                                                    {// Only send when a valve change
                                                        bool checkNeedSendBuf = false;
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (myTimeBuf > (i * groupConfig.timeHoldOn + ((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn * groupConfig.listVS_PLC.Count)))
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                                if ((i + 2 + (2 * groupConfig.listVS_PLC.Count)) > effectSpecialCount)
                                                                {// Only send when valve change
                                                                    effectSpecialCount = (i + 2 + (2 * groupConfig.listVS_PLC.Count));
                                                                    checkNeedSendBuf = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[i].setAngle = groupConfig.endPosition;
                                                            }
                                                        }
                                                        if (checkNeedSendBuf)
                                                        {// Only send if need
                                                            changeVSPLC_Data.Invoke(this, groupConfig);
                                                            effectSpecialCount = 2;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Vẫy lần lượt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước":
                                        if (groupConfig.listVS_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % (long)(((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 2 * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn + groupConfig.timeHoldOff) * groupConfig.listVS_PLC.Count); // 2 times move + (time wait max and min) * number of valve
                                            if (effectSpecialCount == 0)
                                            {// First time process
                                                // Calculator data
                                                effectSpecialCount = 1;
                                                for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                {// Control all valve same
                                                    groupConfig.listVS_PLC[i].enable = true;
                                                    groupConfig.listVS_PLC[i].reset = false;
                                                    groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                    groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                    groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                    groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                    groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                    groupConfig.listVS_PLC[i].setAngle = groupConfig.startPosition;
                                                }
                                                changeVSPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf <= (((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn * groupConfig.listVS_PLC.Count)))
                                                {// in time go to end
                                                    if (effectSpecialCount >= 1)
                                                    {// Only send when a valve change
                                                        bool checkNeedSendBuf = false;
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (myTimeBuf > (i * groupConfig.timeHoldOn))
                                                            {
                                                                groupConfig.listVS_PLC[groupConfig.listVS_PLC.Count - i - 1].setAngle = groupConfig.endPosition;
                                                                if ((i + 2) > effectSpecialCount)
                                                                {// Only send when valve change
                                                                    effectSpecialCount = i + 2;
                                                                    checkNeedSendBuf = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[groupConfig.listVS_PLC.Count - i - 1].setAngle = groupConfig.startPosition;
                                                            }
                                                        }
                                                        if (checkNeedSendBuf)
                                                        {// Only send if need
                                                            changeVSPLC_Data.Invoke(this, groupConfig);
                                                            effectSpecialCount = 2;
                                                        }
                                                    }
                                                }
                                                else
                                                {// in time back to start
                                                    if (effectSpecialCount < (groupConfig.listVS_PLC.Count * 2))
                                                    {// Reload effectSpecialCount
                                                        effectSpecialCount = groupConfig.listVS_PLC.Count * 2;
                                                    }
                                                    if (effectSpecialCount >= groupConfig.listVS_PLC.Count * 2)
                                                    {// Only send when a valve change
                                                        bool checkNeedSendBuf = false;
                                                        for (int i = 0; i < groupConfig.listVS_PLC.Count; i++)
                                                        {// Control all valve same
                                                            groupConfig.listVS_PLC[i].enable = true;
                                                            groupConfig.listVS_PLC[i].reset = false;
                                                            groupConfig.listVS_PLC[i].setCoordAngle = hardwareConfig.optionVS_PLC.coordAngle;
                                                            groupConfig.listVS_PLC[i].setMaxAngle = hardwareConfig.optionVS_PLC.maxAngle;
                                                            groupConfig.listVS_PLC[i].setMaxSpeed = hardwareConfig.optionVS_PLC.maxSpeed;
                                                            groupConfig.listVS_PLC[i].setRatio = hardwareConfig.optionVS_PLC.ratio;
                                                            groupConfig.listVS_PLC[i].setSpeed = groupConfig.speed;
                                                            if (myTimeBuf > (i * groupConfig.timeHoldOn + ((int)(Math.Abs(groupConfig.endPosition - groupConfig.startPosition) * 1000 / groupConfig.speed)) + (groupConfig.timeHoldOn * groupConfig.listVS_PLC.Count)))
                                                            {
                                                                groupConfig.listVS_PLC[groupConfig.listVS_PLC.Count - i - 1].setAngle = groupConfig.startPosition;
                                                                if ((i + 2 + (2 * groupConfig.listVS_PLC.Count)) > effectSpecialCount)
                                                                {// Only send when valve change
                                                                    effectSpecialCount = (i + 2 + (2 * groupConfig.listVS_PLC.Count));
                                                                    checkNeedSendBuf = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVS_PLC[groupConfig.listVS_PLC.Count - i - 1].setAngle = groupConfig.endPosition;
                                                            }
                                                        }
                                                        if (checkNeedSendBuf)
                                                        {// Only send if need
                                                            changeVSPLC_Data.Invoke(this, groupConfig);
                                                            effectSpecialCount = 2;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Thay đổi tự động":

                                    case "Chạy lượn sóng - trái/phải":
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Valve On/Off - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// OFF all led
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// ON all led
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// Blink LED
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listVO_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listVO_PLC[0].setOnOff == false)
                                                        {
                                                            for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                            {// On LED
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            changeVOPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listVO_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listVO_PLC[0].setOnOff == true)
                                                        {
                                                            for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            changeVOPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeHoldOn)); // Cycle each (groupConfig.timeHoldOff + groupConfig.timeHoldOn)
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listVO_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listVO_PLC[0].setOnOff == false)
                                                        {
                                                            for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                            {// On LED
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            changeVOPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listVO_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listVO_PLC[0].setOnOff == true)
                                                        {
                                                            for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            changeVOPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - 1].setOnOff == true)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listVO_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i - 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    if (i == 0)
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = true;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = false;
                                                    }
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - 1].setOnOff == true)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listVO_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i - 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[0].setOnOff == true)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listVO_PLC.Count - 1))
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i + 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[0].setOnOff == true)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listVO_PLC.Count - 1))
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = true;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i + 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// On all LED for first time
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - 1].setOnOff == false)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = groupConfig.listVO_PLC.Count - 1; i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i - 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - 1].setOnOff == false)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listVO_PLC[i].setOnOff = true;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listVO_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i - 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[0].setOnOff == false)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listVO_PLC.Count - 1))
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i + 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = true;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                 // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listVO_PLC[0].setOnOff == false)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listVO_PLC[i].setOnOff = true;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listVO_PLC.Count - 1))
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = false;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i + 1].setOnOff;
                                                            }
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Bật/tắt xen kẽ":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// Toggle all valve - Interleaved 
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = !groupConfig.listVO_PLC[i].setOnOff;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = !groupConfig.listVO_PLC[i - 1].setOnOff;
                                                    }
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Bật/tắt xen kẽ 2":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {// Toggle all valve - Interleaved 
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = !groupConfig.listVO_PLC[i].setOnOff;
                                                    }
                                                    else if (i < 2)
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = groupConfig.listVO_PLC[i - 1].setOnOff;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listVO_PLC[i].setOnOff = !groupConfig.listVO_PLC[i - 2].setOnOff;
                                                    }
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listVO_PLC.Count/3)))
                                                        {
                                                            groupConfig.listVO_PLC[i].setOnOff = true;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listVO_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listVO_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listVO_PLC[i].setOnOff = true;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listVO_PLC[i].setOnOff = false;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listVO_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - 1 lần":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listVO_PLC.Count/3)))
                                                        {
                                                            groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - i - 1].setOnOff = true;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - i - 1].setOnOff = false;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listVO_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - Lặp lại":
                                        if (groupConfig.listVO_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                {
                                                    groupConfig.listVO_PLC[i].setOnOff = false;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listVO_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listVO_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - i - 1].setOnOff = true;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listVO_PLC[groupConfig.listVO_PLC.Count - i - 1].setOnOff = false;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listVO_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Thay đổi tự động":
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Pump Digital - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].setStatus = false;
                                                }
                                                changePumpDigital_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Bật bơm":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// ON all pump
                                                    groupConfig.listInverter[i].setStatus = true;
                                                }
                                                changePumpDigital_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Bật/Tắt bơm - lặp lại":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeHoldOn)); // Cycle each (groupConfig.timeHoldOff + groupConfig.timeHoldOn)
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {
                                                    groupConfig.listInverter[i].setStatus = true;
                                                }
                                                changePumpDigital_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listInverter.Count > 0)
                                                    {
                                                        if (groupConfig.listInverter[0].setStatus == false)
                                                        {
                                                            for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                            {// On LED
                                                                groupConfig.listInverter[i].setStatus = true;
                                                            }
                                                            changePumpDigital_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listInverter.Count > 0)
                                                    {
                                                        if (groupConfig.listInverter[0].setStatus == true)
                                                        {
                                                            for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listInverter[i].setStatus = false;
                                                            }
                                                            changePumpDigital_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Pump Analog - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = false;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changePumpAnalog_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// ON all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changePumpAnalog_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Cố định công suất bơm":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process

                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeGoOn))
                                                {
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                    effectSpecialCount = -1;
                                                }
                                                else
                                                {
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = (int)(groupConfig.freqMax * (musicTime - effectSendByMusicTime) * 100 / groupConfig.timeGoOn);
                                                        if (groupConfig.listInverter[i].manFreq > groupConfig.freqMax)
                                                        {
                                                            groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        }
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Bắn nước - 1 lần":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process

                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeGoOn))
                                                {
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = 0;
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                    effectSpecialCount = -1;
                                                }
                                                else
                                                {
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = (int)(groupConfig.freqMax * (musicTime - effectSendByMusicTime) * 100 / groupConfig.timeGoOn);
                                                        if (groupConfig.listInverter[i].manFreq > groupConfig.freqMax)
                                                        {
                                                            groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        }
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Bắn nước - lặp lại":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeGoOn)); // Cycle each (groupConfig.timeHoldOff + groupConfig.timeGoOn)
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if (myTimeBuf >= ((int)groupConfig.timeGoOn))
                                                {// Wait timeHoldOff
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = 0;
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                }
                                                else
                                                {// In timeGoOn
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = (int)(groupConfig.freqMax * (musicTime - effectSendByMusicTime) * 100 / groupConfig.timeGoOn);
                                                        if (groupConfig.listInverter[i].manFreq > groupConfig.freqMax)
                                                        {
                                                            groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        }
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                }
                                            }
                                        }
                                        break;
                                    case "Thay đổi công suất bơm - lặp lại":
                                        if (groupConfig.listInverter.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeGoOff + groupConfig.timeHoldOn + groupConfig.timeGoOn)); // Cycle each is all time
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;// increase to max
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = groupConfig.freqMin;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) <= ((int)groupConfig.timeGoOn))
                                                {// In timeGoOn
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = (int)(groupConfig.freqMin + ((groupConfig.freqMax - groupConfig.freqMin) * (musicTime - effectSendByMusicTime) / groupConfig.timeGoOn));
                                                        if (groupConfig.listInverter[i].manFreq > groupConfig.freqMax)
                                                        {
                                                            groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        }
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                                else if ((musicTime - effectSendByMusicTime) <= ((int)(groupConfig.timeGoOn + groupConfig.timeHoldOn)))
                                                {// In timeHoldOn
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                    effectSpecialCount = -1;
                                                }
                                                else if ((musicTime - effectSendByMusicTime) <= ((int)(groupConfig.timeGoOn + groupConfig.timeHoldOn)))
                                                {// In timeGoOff
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = (int)(groupConfig.freqMax - ((groupConfig.freqMax - groupConfig.freqMin) * (musicTime - effectSendByMusicTime) / groupConfig.timeGoOn));
                                                        if (groupConfig.listInverter[i].manFreq < groupConfig.freqMin)
                                                        {
                                                            groupConfig.listInverter[i].manFreq = groupConfig.freqMin;
                                                        }
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                                else
                                                {// In timeHoldOff
                                                    // Calculator data
                                                    for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                    {// ON all pump
                                                        groupConfig.listInverter[i].enable = true;
                                                        groupConfig.listInverter[i].setMode = 0;
                                                        groupConfig.listInverter[i].manFreq = groupConfig.freqMin;
                                                        groupConfig.listInverter[i].sinAmplMax = 0;
                                                        groupConfig.listInverter[i].sinAmplMin = 0;
                                                        groupConfig.listInverter[i].sinFreqCycle = 0;
                                                        groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                    effectSpecialCount = -1;
                                                }
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Pump Analog Dual - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// OFF all pump
                                                    groupConfig.listInverter[i].enable = false;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changePumpAnalog_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// ON all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 0;
                                                    groupConfig.listInverter[i].manFreq = groupConfig.freqMax;
                                                    groupConfig.listInverter[i].sinAmplMax = 0;
                                                    groupConfig.listInverter[i].sinAmplMin = 0;
                                                    groupConfig.listInverter[i].sinFreqCycle = 0;
                                                    groupConfig.listInverter[i].sinPhaseDiff = 0;
                                                }
                                                changePumpAnalog_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Tần số lặp lại":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listInverter.Count; i++)
                                                {// Set config for all pump
                                                    groupConfig.listInverter[i].enable = true;
                                                    groupConfig.listInverter[i].setMode = 1;
                                                    groupConfig.listInverter[i].manFreq = 0;
                                                    groupConfig.listInverter[i].sinAmplMax = groupConfig.freqMax;
                                                    groupConfig.listInverter[i].sinAmplMin = groupConfig.freqMin;
                                                    groupConfig.listInverter[i].sinFreqCycle = groupConfig.cycle;
                                                    groupConfig.listInverter[i].sinPhaseDiff = (float)(i * 1.57079632679);
                                                }
                                                changePumpAnalog_Data.Invoke(this, groupConfig);
                                                effectSpecialCount = 0;
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "System Power":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                groupConfig.systemPower = false;
                                                changeSystemPower_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                groupConfig.systemPower = true;
                                                changeSystemPower_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "LED DMX - Artnet":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {// OFF all led
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {// ON all led
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {// Blink LED
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_ArtnetDMX[0].state == 0)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                            {// On LED
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_ArtnetDMX[0].state == 1)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeHoldOn)); // Cycle each (groupConfig.timeHoldOff + groupConfig.timeHoldOn)
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_ArtnetDMX[0].state == 0)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                            {// On LED
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_ArtnetDMX[0].state == 1)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - 1].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_ArtnetDMX.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    if (i == 0)
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                        groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                        groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                    }
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - 1].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_ArtnetDMX.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[0].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_ArtnetDMX.Count - 1))
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[0].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_ArtnetDMX.Count - 1))
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {// On all LED for first time
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - 1].state == 0)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = groupConfig.listLED_ArtnetDMX.Count - 1; i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - 1].state == 0)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_ArtnetDMX.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.listLED_ArtnetDMX[i - 1].red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.listLED_ArtnetDMX[i - 1].green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.listLED_ArtnetDMX[i - 1].blue;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[0].state == 0)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_ArtnetDMX.Count - 1))
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                    groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                    groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                 // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_ArtnetDMX[0].state == 0)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_ArtnetDMX.Count - 1))
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i + 1].state;
                                                                groupConfig.listLED_ArtnetDMX[i].red = groupConfig.listLED_ArtnetDMX[i + 1].red;
                                                                groupConfig.listLED_ArtnetDMX[i].green = groupConfig.listLED_ArtnetDMX[i + 1].green;
                                                                groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.listLED_ArtnetDMX[i + 1].blue;
                                                            }
                                                        }
                                                    }
                                                    changeLEDArtnet_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Nhấp nháy - xen kẽ 1":
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {// Blink LED - Interleaved 
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 1 - groupConfig.listLED_ArtnetDMX[i].state;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red - groupConfig.listLED_ArtnetDMX[i].red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green - groupConfig.listLED_ArtnetDMX[i].green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue - groupConfig.listLED_ArtnetDMX[i].blue;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 1 - groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red - groupConfig.listLED_ArtnetDMX[i - 1].red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green - groupConfig.listLED_ArtnetDMX[i - 1].green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue - groupConfig.listLED_ArtnetDMX[i - 1].blue;
                                                    }
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Nhấp nháy - xen kẽ 2":
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 1 - groupConfig.listLED_ArtnetDMX[i].state;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red - groupConfig.listLED_ArtnetDMX[i].red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green - groupConfig.listLED_ArtnetDMX[i].green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue - groupConfig.listLED_ArtnetDMX[i].blue;
                                                    }
                                                    else if (i < 2)
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = groupConfig.listLED_ArtnetDMX[i - 1].state;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red - groupConfig.listLED_ArtnetDMX[i - 1].red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green - groupConfig.listLED_ArtnetDMX[i - 1].green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue - groupConfig.listLED_ArtnetDMX[i - 1].blue;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_ArtnetDMX[i].state = 1 - groupConfig.listLED_ArtnetDMX[i - 2].state;
                                                        groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red - groupConfig.listLED_ArtnetDMX[i - 2].red;
                                                        groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green - groupConfig.listLED_ArtnetDMX[i - 2].green;
                                                        groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue - groupConfig.listLED_ArtnetDMX[i - 2].blue;
                                                    }
                                                }
                                                changeLEDArtnet_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_ArtnetDMX.Count / 3)))
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_ArtnetDMX.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_ArtnetDMX.Count / 3)))
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[i].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[i].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[i].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = groupConfig.blue;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_ArtnetDMX.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - 1 lần":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_ArtnetDMX.Count / 3)))
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].blue = groupConfig.blue;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].blue = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_ArtnetDMX.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - Lặp lại":
                                        if (groupConfig.listLED_ArtnetDMX.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                {
                                                    groupConfig.listLED_ArtnetDMX[i].state = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].red = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].green = 0;
                                                    groupConfig.listLED_ArtnetDMX[i].blue = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_ArtnetDMX.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_ArtnetDMX.Count / 3)))
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].state = 1;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].red = groupConfig.red;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].green = groupConfig.green;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].blue = groupConfig.blue;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].state = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].red = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].green = 0;
                                                            groupConfig.listLED_ArtnetDMX[groupConfig.listLED_ArtnetDMX.Count - i - 1].blue = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_ArtnetDMX.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        case "LED 485 - PLC":
                            {
                                switch (groupConfig.effectRunning)
                                {
                                    case "Tắt toàn bộ":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {// OFF all led
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở toàn bộ":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            // Calculator data
                                            if ((musicTime - effectSendByMusicTime) >= EFFECT_INTERVAL_RESEND)
                                            {
                                                effectSendByMusicTime = musicTime;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {// ON all led
                                                    groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {// Blink LED
                                                    groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listLED_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_PLC[0].state == 0)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                            {// On LED
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            changeLEDPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listLED_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_PLC[0].state == 1)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            changeLEDPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở rồi tắt - lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            long myTimeBuf = (long)(musicTime - effectSendByMusicTime);
                                            myTimeBuf = myTimeBuf % ((int)(groupConfig.timeHoldOff + groupConfig.timeHoldOn)); // Cycle each (groupConfig.timeHoldOff + groupConfig.timeHoldOn)
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else
                                            {
                                                if (myTimeBuf <= groupConfig.timeHoldOn)
                                                {// In time on
                                                    if (groupConfig.listLED_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_PLC[0].state == 0)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                            {// On LED
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            changeLEDPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                                else
                                                {// In time off
                                                    if (groupConfig.listLED_PLC.Count > 0)
                                                    {
                                                        if (groupConfig.listLED_PLC[0].state == 1)
                                                        {
                                                            for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                            {// Off LED
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            changeLEDPLC_Data.Invoke(this, groupConfig);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - 1].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    if (i == 0)
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 1;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 0;
                                                    }
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0) 
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - 1].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[0].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1; 
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_PLC.Count - 1))
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOn))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[0].state == 1)
                                                    {// OFF all led after all led ON
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_PLC.Count - 1))
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 1;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {// On all LED for first time
                                                    groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - 1].state == 0)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// OFF all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = groupConfig.listLED_PLC.Count - 1; i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                        groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - 1].state == 0)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_PLC[i].state = 1;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = (groupConfig.listLED_PLC.Count - 1); i >= 0; i--)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i - 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                        groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount != -1)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[0].state == 0)
                                                    {// OFF all led after all led OFF, special for one time effect
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                            effectSpecialCount = -1; 
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_PLC.Count - 1))
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                        }
                                        break;
                                    case "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 1;
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {// Normal process
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {// Next effect step
                                                 // Calculator data
                                                    effectSpecialCount++;
                                                    if (groupConfig.listLED_PLC[0].state == 0)
                                                    {// ON all led after all led OFF
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {// ON all led
                                                            groupConfig.listLED_PLC[i].state = 1;
                                                            effectSpecialCount = -1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                        {
                                                            if (i == (groupConfig.listLED_PLC.Count - 1))
                                                            {
                                                                groupConfig.listLED_PLC[i].state = 0;
                                                            }
                                                            else
                                                            {
                                                                groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i + 1].state;
                                                            }
                                                        }
                                                    }
                                                    changeLEDPLC_Data.Invoke(this, groupConfig);
                                                    effectSendByMusicTime = musicTime;
                                                }
                                            }
                                            else
                                            {// Wait off process then repeat
                                                if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Nhấp nháy - xen kẽ 1":
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {// Blink LED - Interleaved 
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 1 - groupConfig.listLED_PLC[i].state;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 1 - groupConfig.listLED_PLC[i - 1].state;
                                                    }
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Nhấp nháy - xen kẽ 2":
                                        {
                                            if ((musicTime - effectSendByMusicTime) >= ((int)groupConfig.timeHoldOff))
                                            {// Next effect step
                                                effectSendByMusicTime = musicTime;
                                                // Calculator data
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    if (i < 1)
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 1 - groupConfig.listLED_PLC[i].state;
                                                    }
                                                    else if (i < 2)
                                                    {
                                                        groupConfig.listLED_PLC[i].state = groupConfig.listLED_PLC[i - 1].state;
                                                    }
                                                    else
                                                    {
                                                        groupConfig.listLED_PLC[i].state = 1 - groupConfig.listLED_PLC[i - 2].state;
                                                    }
                                                }
                                                changeLEDPLC_Data.Invoke(this, groupConfig);
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listLED_PLC[i].state = 1;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Trái-Phải - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listLED_PLC[i].state = 1;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_PLC[i].state = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - 1 lần":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - i - 1].state = 1;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - i - 1].state = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "Chạy lượn sóng Phải-Trái - Lặp lại":
                                        if (groupConfig.listLED_PLC.Count > 0)
                                        {
                                            if (effectSpecialCount == 0)
                                            {// First time process in effect
                                                effectSpecialCount++;
                                                for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                {
                                                    groupConfig.listLED_PLC[i].state = 0;
                                                }
                                                changeVOPLC_Data.Invoke(this, groupConfig);
                                                effectSendByMusicTime = musicTime;
                                            }
                                            else if (effectSpecialCount > 0)
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOn)
                                                {// Next effect step
                                                    effectSendByMusicTime = musicTime;
                                                    // Calculator data
                                                    effectSpecialCount++;
                                                    for (int i = 0; i < groupConfig.listLED_PLC.Count; i++)
                                                    {// Fill data 
                                                        if ((effectSpecialCount > i) && (i > (effectSpecialCount - groupConfig.listLED_PLC.Count / 3)))
                                                        {
                                                            groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - i - 1].state = 1;
                                                        }
                                                        else
                                                        {
                                                            groupConfig.listLED_PLC[groupConfig.listLED_PLC.Count - i - 1].state = 0;
                                                        }
                                                    }
                                                    changeVOPLC_Data.Invoke(this, groupConfig);
                                                    if (effectSpecialCount > (groupConfig.listLED_PLC.Count * 4 / 3))
                                                    {
                                                        effectSpecialCount = -1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((musicTime - effectSendByMusicTime) >= groupConfig.timeHoldOff)
                                                {
                                                    effectSpecialCount = 1;
                                                }
                                            }
                                        }
                                        break;
                                    case "":
                                    case null:
                                        {
                                            // Effect is not set now
                                        }
                                        break;
                                    default:
                                        {
                                            MessageBox.Show("Control_Group_Thread: Unknown runningEffect: " + groupConfig.effectRunning + " .\nClose thread now !!!");
                                            //Thread_Stop();
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            {
                                MessageBox.Show("Control_Group_Thread: Unknown groupType: " + groupConfig.type + ", groupID: " + groupConfig.groupID.ToString() + " .\nClose thread now !!!");
                                //Thread_Stop();
                            }
                            break;
                    }
                    Thread.Sleep(5);
                }
                catch (Exception)
                {

                }
            }
            //MessageBox.Show("Control_Group_Thread of " + groupConfig.name +  " is stopped !!!");
        }
    }
}
