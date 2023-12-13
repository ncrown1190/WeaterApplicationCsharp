using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;

namespace WeaterApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string APIKey = "0b4282d88efa8f5e08d52f1941c2b8e1";

        private void btnSearch_Click(object sender, EventArgs e)
        {
            getWeather();
            getForecast();
        }
       
        // to get the longitude and lattitude for one call api create global variable lon and lat
        double lon;
        double lat;
        void getWeather()
        {
            using (WebClient web = new WebClient()) 
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", TBCity.Text, APIKey);
                //download weather into json object
                var json = web.DownloadString(url);
                //deserialise this into our class our class is WeatherInfo
                WeatherInfo.root Info = JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                //Once we have all the information into info we can use this to update our labels

                picIcon.ImageLocation = "https://openweathermap.org/img/w/" + Info.weather[0].icon + ".png";
                labCondition.Text = Info.weather[0].main;
                labDetails.Text = Info.weather[0].description;
                labSunrise.Text = convertDateTime(Info.sys.sunrise).ToString();
                labSunset.Text = convertDateTime(Info.sys.sunset).ToString();

                labWindSpeed.Text = Info.wind.speed.ToString();
                labPressure.Text = Info.main.pressure.ToString();
                
                // when we get weather for some specific city we expect lat and lon so we get them from this information
                lon = Info.coord.lon;
                lat = Info.coord.lat;
            }
        }

        DateTime convertDateTime(long sec)
        {
            DateTime day =  new DateTime(1970, 1, 1, 0,0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(sec).ToLocalTime();
            return day;
        }

        void getForecast()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/3.0/onecall?lat={0}&lon={1}&exclude=current,minutely,hourly,alerts&appid={2}", lat, lon, APIKey);
                //download weather into json object
                var json = web.DownloadString(url);
                //deserialise this into our class our class is WeatherInfo

                WeatherForecast.ForecastInfo ForecastInfo = JsonConvert.DeserializeObject<WeatherForecast.ForecastInfo>(json);

                /* now we are going to create a user control i.e a pice of windows form which can be
                inserted/display inside a parent form it si not a complete form but a child of the form */

                //Once we have all the information into info we can use this to update our labels

                ForecastUC FUC;

                for(int i = 0; i< 8; i++)
                {
                    FUC = new ForecastUC();
                    FUC.picWeatherIcon.ImageLocation = "https://openweathermap.org/img/w/" + ForecastInfo.daily[i].weather[0].icon + ".png";
                    FUC.labMainWeather.Text = ForecastInfo.daily[i].weather[0].main;
                    FUC.labWeatherDescription.Text = ForecastInfo.daily[i].weather[0].description;
                    FUC.labDT.Text = convertDateTime(ForecastInfo.daily[i].dt).DayOfWeek.ToString();

                    //add the above to FLP i.e flow panel in form1.cs
                    FLP.Controls.Add(FUC);
                }

                               
            }
        }
    }
}
