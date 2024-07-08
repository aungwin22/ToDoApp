using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ToDoApp.Services
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly string _apiKey = "abd7ab7a160c883243c1d8219900c657"; // Replace with your actual API key

        public async Task<string> GetWeatherAsync(string cityName)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_apiKey}&units=metric");

            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadAsStringAsync();
                return ParseWeatherData(weatherData);
            }
            else
            {
                return $"Unable to retrieve weather data for {cityName}. Error: {response.ReasonPhrase}";
            }
        }

        private string ParseWeatherData(string weatherData)
        {
            var json = JObject.Parse(weatherData);
            var description = json["weather"]?[0]?["description"]?.ToString();
            var windSpeed = json["wind"]?["speed"]?.ToString();
            var city = json["name"]?.ToString();

            if (description == null || windSpeed == null || city == null)
            {
                return "Incomplete weather data received.";
            }

            return $"Description: {description}, Wind Speed: {windSpeed} m/s, City: {city}";
        }
    }
}