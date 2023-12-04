using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFountain.Data
{
    public class GLOBAL_CONSTANT
    {
        public const int HARDWARE_OPTION_VALVESTEPPER_COORDANGLE_DEFAULT = 6;
        public const int HARDWARE_OPTION_VALVESTEPPER_MAXANGLE_DEFAULT = 180;
        public const int HARDWARE_OPTION_VALVESTEPPER_MAXSPEED_DEFAULT = 150;
        public const int HARDWARE_OPTION_VALVESTEPPER_RATIO_DEFAULT = 25;

        public const int HARDWARE_OPTION_PUMPANALOG_MAXFREQ_DEFAULT = 50;
        public const int HARDWARE_OPTION_PUMPANALOG_MINFREQ_DEFAULT = 0;

        public const int LOG_TYPE_UNKNOWN = 0;
        public const int LOG_TYPE_SYSTEM_START = 1;
        public const int LOG_TYPE_SYSTEM_STOP = 2;
        public const int LOG_TYPE_MUSIC_START = 11;
        public const int LOG_TYPE_MUSIC_STOP = 12;
    }
}
