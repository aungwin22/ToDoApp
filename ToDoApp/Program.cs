using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Services;

namespace ToDoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var db = new AppDbContext())
            {
                InitDatabase(db);

                var weatherService = new OpenWeatherService();
                var errorLogger = new ErrorLogger();
                var taskService = new TaskService(db, weatherService, errorLogger);
                var uiService = new UserInterfaceService();

                // Upload logs at the start of the application
                await errorLogger.UploadLogsAsync();

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
                            // Upload logs before exiting
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
                WeatherInfo TEXT
            );";

            using (var connection = db.Database.GetDbConnection() as SqliteConnection)
            {
                connection.Open();

                // Check if the ToDoTasks table exists
                var tableExistsCommand = connection.CreateCommand();
                tableExistsCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='ToDoTasks';";
                var tableName = tableExistsCommand.ExecuteScalar();

                if (tableName == null)
                {
                    // Table does not exist, create it
                    var createTableCommand = connection.CreateCommand();
                    createTableCommand.CommandText = createTableScript;
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
