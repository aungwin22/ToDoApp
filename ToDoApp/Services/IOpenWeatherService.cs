using System.Threading.Tasks;

namespace ToDoApp.Services
{
    public interface IOpenWeatherService
    {
        Task<string> GetWeatherAsync(string cityName);
    }
}
