using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;
using ConsoleTables;

namespace ToDoApp.Services
{
    public class TaskService
    {
        private readonly AppDbContext _db;
        private readonly OpenWeatherService _weatherService;
        private readonly ErrorLogger _errorLogger;

        public TaskService(AppDbContext db, OpenWeatherService weatherService, ErrorLogger errorLogger)
        {
            _db = db;
            _weatherService = weatherService;
            _errorLogger = errorLogger;
        }

        public async Task AddTaskAsync()
        {
            try
            {
                Console.Write("Enter title (Maximum 20 characters): ");
                var title = Console.ReadLine();
                if (title.Length > 20)
                {
                    Console.WriteLine("Title cannot be more than 20 characters.");
                    return;
                }

                Console.Write("Enter description (Maximum 45 characters): ");
                var description = Console.ReadLine();
                if (description.Length > 45)
                {
                    Console.WriteLine("Description cannot be more than 45 characters.");
                    return;
                }

                Console.Write("Enter due date (yyyy-MM-dd, e.g., 2024-12-30): ");
                var dueDateInput = Console.ReadLine();
                if (!IsValidDate(dueDateInput, out var dueDate))
                {
                    Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                    return;
                }

                Console.Write("Enter city name for weather information (if your city name is not listed, weather information may not show correctly): ");
                var cityName = Console.ReadLine();
                string weatherInfo = await _weatherService.GetWeatherAsync(cityName);

                var task = new ToDoTask
                {
                    Title = title,
                    Description = description,
                    DueDate = dueDate,
                    IsCompleted = false,
                    WeatherInfo = weatherInfo
                };

                _db.ToDoTasks.Add(task);
                await _db.SaveChangesAsync();
                Console.WriteLine("Task added successfully.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                _errorLogger.LogError("Error adding task", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                _errorLogger.LogError("Unexpected error adding task", ex);
            }
        }



        public void ViewTasks()
        {
            try
            {
                var tasks = _db.ToDoTasks.OrderBy(t => t.DueDate).ToList();
                var table = new ConsoleTable("Id", "Title", "Description", "Due Date", "Completed", "WeatherInfo");

                foreach (var task in tasks)
                {
                    table.AddRow(task.Id, task.Title, task.Description, task.DueDate.ToString("yyyy-MM-dd"), task.IsCompleted ? "Yes" : "No", task.WeatherInfo);
                }

                table.Write();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void MarkTaskAsCompleted()
        {
            try
            {
                Console.Write("Enter task Id to mark as completed: ");
                var id = int.Parse(Console.ReadLine());

                var task = _db.ToDoTasks.Find(id);
                if (task != null)
                {
                    task.IsCompleted = true;
                    _db.SaveChanges();
                    Console.WriteLine("Task marked as completed.");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void RemoveTask()
        {
            try
            {
                Console.Write("Enter task Id to remove: ");
                var id = int.Parse(Console.ReadLine());

                var task = _db.ToDoTasks.Find(id);
                if (task != null)
                {
                    _db.ToDoTasks.Remove(task);
                    _db.SaveChanges();
                    Console.WriteLine("Task removed.");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void EditTask()
        {
            try
            {
                Console.Write("Enter task Id to edit: ");
                var id = int.Parse(Console.ReadLine());

                var task = _db.ToDoTasks.Find(id);
                if (task != null)
                {
                    Console.Write("Enter new title (leave blank to keep current, Maximum 20 characters): ");
                    var title = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title) && title.Length > 20)
                    {
                        Console.WriteLine("Title cannot be more than 20 characters.");
                        return;
                    }

                    Console.Write("Enter new description (leave blank to keep current, Maximum 45 characters): ");
                    var description = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(description) && description.Length > 45)
                    {
                        Console.WriteLine("Description cannot be more than 45 characters.");
                        return;
                    }

                    Console.Write("Enter new due date (leave blank to keep current, yyyy-MM-dd, e.g., 2024-12-30): ");
                    var dueDateInput = Console.ReadLine();
                    DateTime dueDate = task.DueDate;
                    if (!string.IsNullOrWhiteSpace(dueDateInput))
                    {
                        if (!IsValidDate(dueDateInput, out dueDate))
                        {
                            Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                            return;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(title))
                        task.Title = title;
                    if (!string.IsNullOrWhiteSpace(description))
                        task.Description = description;
                    if (!string.IsNullOrWhiteSpace(dueDateInput))
                        task.DueDate = dueDate;

                    _db.SaveChanges();
                    Console.WriteLine("Task updated.");
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void FilterTasksByCompletionStatus()
        {
            try
            {
                Console.Write("Enter completion status (yes/no): ");
                var statusInput = Console.ReadLine().ToLower();
                bool status;

                if (statusInput == "yes")
                {
                    status = true;
                }
                else if (statusInput == "no")
                {
                    status = false;
                }
                else
                {
                    Console.WriteLine("Invalid status. Please enter 'yes' or 'no'.");
                    return;
                }

                var tasks = _db.ToDoTasks.Where(t => t.IsCompleted == status).OrderBy(t => t.DueDate).ToList();
                var table = new ConsoleTable("Id", "Title", "Description", "Due Date", "Completed", "WeatherInfo");

                foreach (var task in tasks)
                {
                    table.AddRow(task.Id, task.Title, task.Description, task.DueDate.ToString("yyyy-MM-dd"), task.IsCompleted ? "Yes" : "No", task.WeatherInfo);
                }

                table.Write();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void FilterTasksByDateRange()
        {
            try
            {
                Console.Write("Enter start date (yyyy-MM-dd): ");
                var startDateInput = Console.ReadLine();
                if (!IsValidDate(startDateInput, out var startDate))
                {
                    Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                    return;
                }

                Console.Write("Enter end date (yyyy-MM-dd): ");
                var endDateInput = Console.ReadLine();
                if (!IsValidDate(endDateInput, out var endDate))
                {
                    Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                    return;
                }

                var tasks = _db.ToDoTasks.Where(t => t.DueDate >= startDate && t.DueDate <= endDate).OrderBy(t => t.DueDate).ToList();
                var table = new ConsoleTable("Id", "Title", "Description", "Due Date", "Completed", "WeatherInfo");

                foreach (var task in tasks)
                {
                    table.AddRow(task.Id, task.Title, task.Description, task.DueDate.ToString("yyyy-MM-dd"), task.IsCompleted ? "Yes" : "No", task.WeatherInfo);
                }

                table.Write();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"An error occurred while updating the database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public bool IsValidDate(string dateInput, out DateTime date)
        {
            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                if (date.Year < 1 || date.Year > 9999 || date.Month < 1 || date.Month > 12 || date.Day < 1 || date.Day > DateTime.DaysInMonth(date.Year, date.Month))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
