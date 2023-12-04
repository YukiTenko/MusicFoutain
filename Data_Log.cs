using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MusicFountain.Data
{
    public class Data_Log_Info
    {
        public int type { get; set; }
        public string time { get; set; }
        public string text { get; set; }
    }
    public class Data_Log_One_Day
    {
        public string date { get; set; }
        public List<Data_Log_Info> listLogs { get; set; }
    }
    
    // Interface class
    class Data_Log
    {
        public Data_Log()
        {
            {// Creat variables
            }

            {// Read list user from Login.json
            }
        }

        // Public methods
        public List<Data_Log_Info> GetLogByPath(string newLogFilePath)
        {
            if (File.Exists(newLogFilePath))
            {// Read data and deserialize
                string jsonStrBuf = File.ReadAllText(newLogFilePath);
                try
                {
                    Data_Log_One_Day jsonLogBuf = JsonSerializer.Deserialize<Data_Log_One_Day>(jsonStrBuf);
                    return jsonLogBuf.listLogs;
                }
                catch (Exception)
                {
                    MessageBox.Show("Data_Login: Can not deserialize data: " + jsonStrBuf + " to JSON");
                }
            }
            return null;
        }
        public int CreatLog(int type, string text)
        {
            string filePathBuf = "../Data/Log/" + DateTime.Now.ToString("dd-MM-yyyy") + ".json";
            if (File.Exists(filePathBuf))
            {// Read data and deserialize
                string jsonStrBuf = File.ReadAllText(filePathBuf);
                try
                {
                    Data_Log_One_Day jsonLogBuf = JsonSerializer.Deserialize<Data_Log_One_Day>(jsonStrBuf);
                    Data_Log_Info newLogBuf = new Data_Log_Info();
                    newLogBuf.time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    newLogBuf.type = type;
                    newLogBuf.text = text;
                    jsonLogBuf.listLogs.Add(newLogBuf);

                    {// Write hardware config to file
                        if (File.Exists(filePathBuf))
                        {// Read data and deserialize
                            File.WriteAllText(filePathBuf, JsonSerializer.Serialize(jsonLogBuf));
                        }
                        else
                        {
                            MessageBox.Show("Data_Log: Can not find file with path: " + filePathBuf);
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Data_Login: Can not deserialize data: " + jsonStrBuf + " to JSON");
                    return -1;
                }
            }
            else
            {
                FileStream fileCreatBuf = File.Create(filePathBuf);
                if (fileCreatBuf != null)
                {
                    fileCreatBuf.Close();

                    Data_Log_One_Day jsonLogBuf = new Data_Log_One_Day();
                    jsonLogBuf.date = DateTime.Now.ToString("dd-MM-yyyy");
                    jsonLogBuf.listLogs = new List<Data_Log_Info>();

                    Data_Log_Info newLogBuf = new Data_Log_Info();
                    newLogBuf.time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    newLogBuf.type = type;
                    newLogBuf.text = text;
                    jsonLogBuf.listLogs.Add(newLogBuf);

                    {// Write hardware config to file
                        if (File.Exists(filePathBuf))
                        {// Read data and deserialize
                            File.WriteAllText(filePathBuf, JsonSerializer.Serialize(jsonLogBuf));
                        }
                        else
                        {
                            MessageBox.Show("Data_Log: Can not find file with path: " + filePathBuf);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Tạo file mới không thành công\nĐường dẫn: " + filePathBuf);
                }
            }
            return 0;
        }
    
        public int DeleteAllLog()
        {
            string[] listLogFilesBuf = Directory.GetFiles("../Data/Log");
            foreach (string logFileBuf in listLogFilesBuf)
            {
                File.Delete(logFileBuf);
            }

            CreatLog(GLOBAL_CONSTANT.LOG_TYPE_UNKNOWN, "Đã xoá lịch sử hoạt động");
            return 0;
        }
    }
}
