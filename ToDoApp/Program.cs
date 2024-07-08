using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Data;
using ToDoApp.Services;

namespace ToDoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Check if appsettings.json file exists
            var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Configuration file 'appsettings.json' not found.");
                return;
            }

            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            using (var db = new AppDbContext())
            {
                // Initialize the database and create tables if they do not exist
                InitDatabase(db);

                // Read API key and base URL from configuration
                var apiKey = configuration["OpenWeather:ApiKey"];
                var baseUrl = configuration["OpenWeather:BaseUrl"];
                var httpClient = new HttpClient();

                // Initialize services
                var weatherService = new OpenWeatherService(apiKey, baseUrl, httpClient);
                var errorLogger = new ErrorLogger();
                var validationService = new ValidationService();
                var taskRepository = new TaskRepository(db);
                var taskService = new TaskService(taskRepository, weatherService, errorLogger, validationService);
                var uiService = new UserInterfaceService();

                // Upload logs at the start of the application
                await errorLogger.UploadLogsAsync();

                // Main application loop
                while (true)
                {
                    uiService.ShowMenu();
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await taskService.AddTaskAsync();
                            break;
                        case "2":
                            taskService.ViewTasks();
                            break;
                        case "3":
                            taskService.MarkTaskAsCompleted();
                            break;
                        case "4":
                            taskService.RemoveTask();
                            break;
                        case "5":
                            taskService.EditTask();
                            break;
                        case "6":
                            taskService.FilterTasksByCompletionStatus();
                            break;
                        case "7":
                            taskService.FilterTasksByDateRange();
                            break;
                        case "8":
                            Console.WriteLine("Exiting the application. Press any key to close the console...");
                            Console.ReadKey();
                            await errorLogger.UploadLogsAsync();
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
        }

        static void InitDatabase(AppDbContext db)
        {
            string createTableScript = @"
        CREATE TABLE IF NOT EXISTS ToDoTasks (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Description TEXT NOT NULL,
            DueDate TEXT NOT NULL,
            IsCompleted BOOLEAN NOT NULL,
            WeatherInfo TEXT,
            City TEXT );";

            using (var connection = db.Database.GetDbConnection() as SqliteConnection)
            {
                connection.Open();

                var tableExistsCommand = connection.CreateCommand();
                tableExistsCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='ToDoTasks';";
                var tableName = tableExistsCommand.ExecuteScalar();

                if (tableName == null)
                {
                    var createTableCommand = connection.CreateCommand();
                    createTableCommand.CommandText = createTableScript;
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
