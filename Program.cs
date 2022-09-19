using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace WeatherFetch
{

    class Program
    {
        static void Main(string[] args)
        {
            string settingsPath = "";
            string EndDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");         
            int Days = 0;
            
            //Obtain number of days past to fetch weather data
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().Contains("-days=") == true)
                {                    
                    try
                    {
                        Days = int.Parse(args[i].Replace("-days=", "").Trim())-1;
                        if ((Days<0)&&(Days>365))
                        {
                           Days = 0;
                        }
                    }
                    catch
                    {
                        Days = 0;
                    }
                }

                if (args[i].ToLower().Contains("-settingspath=") == true)
                {
                    try
                    {
                        settingsPath = args[i].Replace("-settingspath=", "").Trim();

                    }
                    catch
                    {
                        settingsPath = "";
                    }
                }

            }


            //Days = 4;

            //Read Settings.json file
            var settingsJSON = System.IO.File.ReadAllText(settingsPath + "Settings.json");

            string DeviceMACAddress = (string)JObject.Parse(settingsJSON)["DeviceMACAddress"];
            string ApiKey = (string)JObject.Parse(settingsJSON)["ApiKey"];
            string ApplicationKey = (string)JObject.Parse(settingsJSON)["ApplicationKey"];
            string dbPath = (string)JObject.Parse(settingsJSON)["dbPath"].ToString().Trim();


            //Iterate thru days past and fetch weather data from AmbientWeather Service
            for (int i = 0; i<=Days;i++)
            {
                string NewEndDate = DateTime.Parse(EndDate).AddDays(-1*i).ToString("yyyy-MM-dd");
                string status = FetchReadings(DeviceMACAddress, ApiKey, ApplicationKey, NewEndDate, dbPath);
                Console.WriteLine(status);
            }

            System.Environment.Exit(0);
        }

        public static string FetchReadings(string DeviceMACAddress, string ApiKey, string ApplicationKey, string EndDate, string dbPath)
        {
            int ReadingsFound = 0;
            int ReadingsSaved = 0;

            System.Threading.Thread.Sleep(5000); //Ambient Weather 1 second delay for API usage

            string response = "";
            response = GetReadings(@"https://api.ambientweather.net/v1/devices/" + DeviceMACAddress + "?apiKey=" + ApiKey + "&applicationKey=" + ApplicationKey + "&endDate=" + EndDate);

            List<DeviceData> AmbientWeather = JsonConvert.DeserializeObject<List<DeviceData>>(response);


            foreach (DeviceData Reading in AmbientWeather)
            {
                Microsoft.Data.Sqlite.SqliteConnection m_dbConnection = new SqliteConnection("Data Source=" + dbPath + "WeatherStation.sqlite;");
                m_dbConnection.Open();

                string LocalDateTime = "";
                try
                {
                    LocalDateTime = DateTime.Parse(Reading.date).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                }
                catch
                {
                    LocalDateTime = "";
                }
                    ReadingsFound++;

                string insertReadingsSql = "insert into Readings values "
                  + "(" + Reading.dateutc
                  + "," + Reading.winddir
                  + "," + Reading.windspeedmph
                  + "," + Reading.windgustmph
                  + "," + Reading.maxdailygust
                  + "," + Reading.tempf
                  + "," + Reading.hourlyrainin
                  + "," + Reading.eventrainin
                  + "," + Reading.dailyrainin
                  + "," + Reading.weeklyrainin
                  + "," + Reading.monthlyrainin
                  + "," + Reading.totalrainin
                  + "," + Reading.baromrelin
                  + "," + Reading.baromabsin
                  + "," + Reading.humidity
                  + "," + Reading.tempinf
                  + "," + Reading.humidityin
                  + "," + Reading.uv
                  + "," + Reading.solarradiation
                  + "," + Reading.feelsLike
                  + "," + Reading.dewPoint
                  + ",'" + Reading.lastRain + "'"
                  + ",'" + Reading.date + "'"
                  + ",'" + LocalDateTime + "')";


                Microsoft.Data.Sqlite.SqliteCommand insertReadings = new SqliteCommand(insertReadingsSql, m_dbConnection);

               try
               {
                    insertReadings.ExecuteNonQuery();
                    m_dbConnection.Close();
                    ReadingsSaved++;
               }
                catch(Exception e)
               {
                    m_dbConnection.Close();
               }
            }

            return DateTime.Parse(EndDate).AddDays(-1).ToString("yyyy-MM-dd") + " - Reading Found=" + ReadingsFound + " Saved=" + ReadingsSaved;
 
        }

                

        public static string GetReadings(string url)
        {
            var client = new WebClient();

            var response = client.DownloadString(url);

            return response;
        }


        public class DeviceData
        {
            public object dateutc { get; set; }
            public int winddir { get; set; }
            public double windspeedmph { get; set; }
            public double windgustmph { get; set; }
            public double maxdailygust { get; set; }
            public double tempf { get; set; }
            public double hourlyrainin { get; set; }
            public double eventrainin { get; set; }
            public double dailyrainin { get; set; }
            public double weeklyrainin { get; set; }
            public double monthlyrainin { get; set; }
            public double totalrainin { get; set; }
            public double baromrelin { get; set; }
            public double baromabsin { get; set; }
            public int humidity { get; set; }
            public double tempinf { get; set; }
            public int humidityin { get; set; }
            public int uv { get; set; }
            public double solarradiation { get; set; }
            public double feelsLike { get; set; }
            public double dewPoint { get; set; }
            public string lastRain { get; set; }
            public string date { get; set; }
        }

    }    
}
