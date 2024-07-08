using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ToDoApp.Services
{
    public class OpenWeatherService
    {
        private readonly string _apiKey = "d3e66c6e3cc3ecf0d055bb6b57b6e754"; // Replace with your OpenWeather API key
        

        public async Task<string> GetWeatherAsync(string location)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}");
                var weatherData = JObject.Parse(response);
                var weatherDescription = weatherData["weather"][0]["description"].ToString();
                var temperature = weatherData["main"]["temp"].ToString();
                return $"Weather: {weatherDescription}, Temp: {temperature}K";
            }
        }
    }
}
