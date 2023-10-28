using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MusicFountain
{
    public class Data_UserInfo
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
    public class List_UserInfo
    {
        public List<Data_UserInfo> listUser { get; set; }
    }

    public partial class Data_Login
    {
        // Variables
        List_UserInfo listUser;

        public Data_Login()
        {
            {// Creat variables
                listUser = new List_UserInfo();
            }

            {// Read list user from Login.json
                string filePath = "../Data/Login.json";
                if (File.Exists(filePath))
                {// Read data and deserialize
                    string jsonStrBuf = File.ReadAllText(filePath);
                    try
                    {
                        listUser = JsonSerializer.Deserialize<List_UserInfo>(jsonStrBuf);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Data_Login: Can not deserialize data: " + jsonStrBuf + " to JSON");
                    }
                }
                else
                {
                    MessageBox.Show("Data_Login: Can not find file with path: " + filePath);
                }
            }
        }

        public bool Compare_UserNameAndPassword(string userName, string password)
        {
            for (int i = 0; i < listUser.listUser.Count; i++)
            {
                if ((userName == listUser.listUser[i].userName) && (password == listUser.listUser[i].password))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
