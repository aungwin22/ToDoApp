using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ToDoApp.Services
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public OpenWeatherService(string apiKey, string baseUrl, HttpClient httpClient)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        public async Task<string> GetWeatherAsync(string cityName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}weather?q={cityName}&appid={_apiKey}&units=metric");

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
