using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Independentsoft.Office.Odf;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Globalization;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //parameter///////////////////////////////////////////////////////////
            int counter = 1;
            string line;
            string txtFile = "http://www.caa.gov.tw/APFile/big5/TimeSheet/GET_SCHE_PUB_DOM.txt";
            string AirWay, FilghtID, GCity, FCity, Fday, FigetType1, FigetType2;
            DateTime Dtime, Atime, Dday, Aday;
            int GID = 0,FID=0;           
            bool SUN = false, MON = false, TUE = false, WED = false, THU = false, FRI = false, SAT = false;
            string SQL = null,DDday= null,AADay = null;
            int n=0;
            //Connect MySql///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string dbHost = "127.0.0.1";
            string dbUser = "root";
            string dbPass = "welz1738";
            string dbName = "airalliance";
            string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            MySqlConnection conn = new MySqlConnection(connStr);
            // 連線到資料庫
            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("無法連線到資料庫.");
                        break;
                    case 1045:
                        Console.WriteLine("使用者帳號或密碼錯誤,請再試一次.");
                        break;
                }
            }
            //END Connect MySql/

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(txtFile);
                StreamReader file = new StreamReader(stream);

                while ((line = file.ReadLine()) != null)
                {
                    char delimiter = ',';
                    String[] substrings = line.Split(delimiter);//分割資訊

                    AirWay = Convert.ToString(substrings[0]);//航空公司
                    FilghtID = Convert.ToString(substrings[1]);//航班代碼
                    GCity = Convert.ToString(substrings[3]);//起飛機場
                    FCity = Convert.ToString(substrings[20]);//降落機場
                    Fday = Convert.ToString(substrings[24]);//周幾有班
                    FigetType1 = Convert.ToString(substrings[25]);//機型1
                    FigetType2 = Convert.ToString(substrings[26]);//機型2

                    Dtime = Convert.ToDateTime(substrings[4].Insert(2, ":"));//起飛時間
                    Atime = Convert.ToDateTime(substrings[21].Insert(2, ":"));//降落時間
                  
                    Dday = Convert.ToDateTime(substrings[22]);//
                    Aday = Convert.ToDateTime(substrings[23]);//飛機起飛日期
                    DDday =Dday.ToString("yyyy-MM-dd");
                    AADay = Aday.ToString("yyyy-MM-dd");

                    TimeSpan TS = new TimeSpan(Dtime.Ticks);
                    TimeSpan TE = new TimeSpan(Atime.Ticks);
                    TimeSpan T = TS.Subtract(TE).Duration();//飛行時間

                    //周幾有飛//
                    for (int i = 6; i > 0; i--) { Fday = Fday.Insert(i, ","); }
                    String[] FD = Fday.Split(delimiter);
                    for (int j = 0; j < 8; j++) {Week(ref SUN, ref MON, ref TUE, ref WED, ref THU, ref FRI, ref SAT, j);}
                 
                    //寫入MYSQL STRART//
                    GID = IDFunc(GCity, GID);
                    FID =IDFunc(FCity, FID);
                   SQL = "INSERT INTO `schedule` ( `GID`, `FID`, `Date`)VALUES("+GID+","+FID+ ",'" +DDday+"');";
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                     n = cmd.ExecuteNonQuery();
                    //寫入MYSQL END//

                    counter++;
                }//While end
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            System.Console.WriteLine("There were {0} lines.", counter);
            System.Console.ReadLine();


            // mySql select
            SQL = "SELECT * FROM flights WHERE 1";
            
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                MySqlDataReader myData = cmd.ExecuteReader();

                if (!myData.HasRows)
                {
                    Console.WriteLine("No data.");
                }
                else
                {
                    while (myData.Read())
                    {
                        Console.WriteLine("Text={0}", myData.GetString(0));
                    }
                    Console.ReadKey();
                    myData.Close();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
            }

            conn.Close();
        }

        private static void Week(ref bool SUN, ref bool MON, ref bool TUE, ref bool WED, ref bool THU, ref bool FRI, ref bool SAT, int j)
        {
            switch (j)
            {
                case 1:
                    MON = true;
                    break;
                case 2:
                    TUE = true;
                    break;
                case 3:
                    WED = true;
                    break;
                case 4:
                    THU = true;
                    break;
                case 5:
                    FRI = true;
                    break;
                case 6:
                    SAT = true;
                    break;
                case 7:
                    SUN = true;
                    break;
                default:
                    break;
            }
        }

        private static int IDFunc(string City, int ID)
        {
            switch (City)
            {
                case "TPE":
                    ID = 1;
                    break;
                case "CYI":
                    ID = 2;
                    break;
                case "CMJ":
                    ID = 3;
                    break;
                case "GNI":
                    ID = 4;
                    break;
                case "HUN":
                    ID = 5;
                    break;
                case "KHH":
                    ID = 6;
                    break;
                case "KNH":
                    ID = 7;
                    break;
                case "MZG":
                    ID = 8;
                    break;
                case "MFK":
                    ID = 9;
                    break;
                case "KYD":
                    ID = 10;
                    break;
                case "PIF":
                    ID = 11;
                    break;
                case "WOT":
                    ID = 12;
                    break;
                case "TSA":
                    ID = 13;
                    break;
                case "TXG":
                    ID = 14;
                    break;
                case "TTT":
                    ID = 15;
                    break;
                case "TNN":
                    ID = 16;
                    break;
                default:
                    ID = 0;
                    break;
            }
            return ID;
        }

    }
}

