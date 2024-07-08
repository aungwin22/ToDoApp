using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;
using Xunit;

namespace ToDoApp.Tests.Services
{
    public class TaskServiceTests : IDisposable
    {
        private readonly AppDbContext _context;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task InsertTask_Should_Add_Task_To_Database()
        {
            // Arrange
            var task = new ToDoTask
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now,
                IsCompleted = false,
                WeatherInfo = "Sunny",
                City = "Melbourne"
            };

            // Act
            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();

            // Assert
            var tasks = await _context.ToDoTasks.ToListAsync();
            Assert.Single(tasks);
            Assert.Equal("Test Task", tasks[0].Title);
        }

        [Fact]
        public async Task ReadTask_Should_Return_Existing_Task_From_Database()
        {
            // Arrange
            var task = new ToDoTask
            {
                Title = "Task to Read",
                Description = "Description",
                DueDate = DateTime.Now,
                IsCompleted = false,
                WeatherInfo = "Sunny",
                City = "Melbourne"
            };
            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var retrievedTask = await _context.ToDoTasks.FindAsync(task.Id);

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Title, retrievedTask.Title);
        }

        [Fact]
        public async Task UpdateTask_Should_Modify_Existing_Task_In_Database()
        {
            // Arrange
            var task = new ToDoTask
            {
                Title = "Initial Title",
                Description = "Initial Description",
                DueDate = DateTime.Now,
                IsCompleted = false,
                WeatherInfo = "Sunny",
                City = "Melbourne"
            };
            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            task.Title = "Updated Title";
            task.Description = "Updated Description";
            _context.ToDoTasks.Update(task);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTask = await _context.ToDoTasks.FirstAsync();
            Assert.Equal("Updated Title", updatedTask.Title);
            Assert.Equal("Updated Description", updatedTask.Description);
        }

        [Fact]
        public async Task DeleteTask_Should_Remove_Task_From_Database()
        {
            // Arrange
            var task = new ToDoTask
            {
                Title = "Task to Delete",
                Description = "Description",
                DueDate = DateTime.Now,
                IsCompleted = false,
                WeatherInfo = "Sunny",
                City = "Melbourne"
            };
            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            _context.ToDoTasks.Remove(task);
            await _context.SaveChangesAsync();

            // Assert
            var tasks = await _context.ToDoTasks.ToListAsync();
            Assert.Empty(tasks);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
