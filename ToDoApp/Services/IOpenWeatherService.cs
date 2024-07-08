using System.Threading.Tasks;

namespace ToDoApp.Services
{
    /// <summary>
    /// Interface for a service that retrieves weather information.
    /// </summary>
    public interface IOpenWeatherService
    {
        Task<string> GetWeatherAsync(string cityName);
    }
}
