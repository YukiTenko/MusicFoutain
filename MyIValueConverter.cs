using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicFountain.UI
{
    public partial class DeviceListConverter : IValueConverter
    {
        public List<string> LIST_VALVE_STEP_PLC = new List<string>() { "Van 1", "Van 2", "Van 3", "Van 4" };
        public List<string> LIST_VALVE_ONOFF_PLC = new List<string>() { "Van 1", "Van 2", "Van 3", "Van 4", "Van 5", "Van 6", "Van 7", "Van 8", "Van 9", "Van 10", "Van 11", "Van 12", "Van 13", "Van 14", "Van 15", "Van 16" };
        public List<string> LIST_PUMP_DIGITAL_PLC = new List<string>() { "Bơm 1", "Bơm 2", "Bơm 3", "Bơm 4", "Bơm 5"};
        public List<string> LIST_PUMP_ANALOG_PLC = new List<string>() { "Biến tần 1", "Biến tần 2", "Biến tần 3", "Biến tần 4"};
        public List<string> LIST_DEVICE_UNKNOWN = new List<string>() { };

        public object Convert(object value, Type targetType,
                                object parameter, CultureInfo culture)
        {
            string key = value as string;
            switch (key)
            {
                case "Valve Stepper - PLC":
                    return LIST_VALVE_STEP_PLC;

                case "Valve On/Off - PLC":
                    return LIST_VALVE_ONOFF_PLC;

                case "LED 485 - PLC":
                    List<string> LIST_LED485_PLC = new List<string>();
                    LIST_LED485_PLC.Clear();
                    for (int i = 0; i < 500; i++)
                    {
                        LIST_LED485_PLC.Add("LED " + (i+1).ToString());
                    }
                    return LIST_LED485_PLC;

                case "Pump Digital - PLC":
                    return LIST_PUMP_DIGITAL_PLC;

                case "Pump Analog - PLC":
                case "Pump Analog Dual - PLC":
                    return LIST_PUMP_ANALOG_PLC;

                default: return LIST_DEVICE_UNKNOWN;
            }
        }
        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public partial class EffectListConverter : IValueConverter
    {
        public List<string> LIST_EFFECT_LED = new List<string>() { "Tắt toàn bộ", "Mở toàn bộ", "Mở rồi tắt - 1 lần", "Mở rồi tắt - lặp lại", "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần", "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần", "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần", "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần", "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại", "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại", "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại", "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại", "Nhấp nháy - xen kẽ 1", "Nhấp nháy - xen kẽ 2" };
        public List<string> LIST_EFFECT_VALVE_PLC = new List<string>() { "Tắt toàn bộ", "Mở toàn bộ", "Mở rồi tắt - 1 lần", "Mở rồi tắt - lặp lại", "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần", "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần", "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - 1 lần", "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - 1 lần", "Chạy lượn sóng Trái-Phải - 1 lần", "Chạy lượn sóng Phải-Trái - 1 lần", "Mở Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại", "Mở Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại", "Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại", "Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại", "Chạy lượn sóng Trái-Phải - Lặp lại", "Chạy lượn sóng Phải-Trái - Lặp lại", "Bật/tắt xen kẽ", "Bật/tắt xen kẽ 2" };
        public List<string> LIST_EFFECT_STEPPER_PLC = new List<string>() { "Không vẫy", "Vẫy đồng thời", "Vẫy đồng thời - xen kẽ 1 van", "Vẫy đồng thời - xen kẽ 2 van", "Vẫy lần lượt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau", "Vẫy lần lượt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước", "Tắt toàn bộ", "Về gốc tọa độ", "Hai nửa đối xứng" };
        public List<string> LIST_EFFECT_PUMP_ANALOG_PLC = new List<string>() { "Tắt toàn bộ", "Cố định công suất bơm", "Thay đổi công suất bơm - lặp lại", "Bắn nước - 1 lần", "Bắn nước - lặp lại" };
        public List<string> LIST_EFFECT_PUMP_ANALOG2_PLC = new List<string>() { "Tắt toàn bộ", "Cố định công suất bơm", "Thay đổi công suất bơm - lặp lại" };
        public List<string> LIST_EFFECT_PUMP_DIGITAL_PLC = new List<string>() { "Tắt toàn bộ", "Bật bơm", "Bật/Tắt bơm - lặp lại" };
        public List<string> LIST_EFFECT_SYSTEMPOWER = new List<string>() { "Tắt toàn bộ", "Mở toàn bộ" };
        public List<string> LIST_EFFECT_UNKNOWN = new List<string>() { "Unknown" };

        public object Convert(object value, Type targetType,
                                object parameter, CultureInfo culture)
        {
            string key = value as string;
            switch (key)
            {
                case "Valve Stepper - PLC":
                    return LIST_EFFECT_STEPPER_PLC;

                case "Valve On/Off - PLC":
                    return LIST_EFFECT_VALVE_PLC;

                case "LED 485 - PLC":
                case "LED DMX - Artnet":
                    return LIST_EFFECT_LED;

                case "Pump Digital - PLC":
                    return LIST_EFFECT_PUMP_DIGITAL_PLC;

                case "Pump Analog - PLC":
                    return LIST_EFFECT_PUMP_ANALOG_PLC;

                case "Pump Analog Dual - PLC":
                    return LIST_EFFECT_PUMP_ANALOG2_PLC;

                case "System Power":
                    return LIST_EFFECT_SYSTEMPOWER;

                default: return LIST_EFFECT_UNKNOWN;
            }
        }
        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    
    public partial class ColorTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int key = ((int)value)%6;
            switch (key)
            {
                case 1:
                  return  Brushes.LightBlue;
                case 2:
                    return Brushes.LightYellow;
                case 3:
                    return Brushes.LightCyan;
                case 4:
                    return Brushes.LightGray;
                case 5:
                    return Brushes.LightGreen;
                case 0:
                default:
                    return Brushes.GhostWhite;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public partial class ColorLogTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int logType = (int)value;
            switch (logType)
            {
                case Data.GLOBAL_CONSTANT.LOG_TYPE_SYSTEM_START:
                case Data.GLOBAL_CONSTANT.LOG_TYPE_SYSTEM_STOP:
                    return Brushes.LightSkyBlue;
                case Data.GLOBAL_CONSTANT.LOG_TYPE_MUSIC_START:
                case Data.GLOBAL_CONSTANT.LOG_TYPE_MUSIC_STOP:
                    return Brushes.LightGreen;
                case Data.GLOBAL_CONSTANT.LOG_TYPE_UNKNOWN:
                default:
                    return Brushes.LightGray;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                    object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
