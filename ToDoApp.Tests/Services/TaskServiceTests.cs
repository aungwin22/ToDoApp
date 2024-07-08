using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Services;
using Xunit;

namespace ToDoApp.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly Mock<OpenWeatherService> _mockWeatherService;
        private readonly Mock<ErrorLogger> _mockErrorLogger;

        public TaskServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ToDoAppTestDb")
                .Options;

            _mockWeatherService = new Mock<OpenWeatherService>();
            _mockErrorLogger = new Mock<ErrorLogger>();
        }

        private AppDbContext CreateContext() => new AppDbContext(_dbContextOptions);

        [Fact]
        public async Task AddTaskAsync_Should_Add_Task_To_Database()
        {
            // Arrange
            using var context = CreateContext();
            var taskService = new TaskService(context, _mockWeatherService.Object, _mockErrorLogger.Object);

            _mockWeatherService.Setup(s => s.GetWeatherAsync(It.IsAny<string>()))
                .ReturnsAsync("Sunny");

            // Act
            await taskService.AddTaskAsync();

            // Assert
            var tasks = await context.ToDoTasks.ToListAsync();
            Assert.Single(tasks);
        }

        [Fact]
        public async Task AddTaskAsync_Should_Log_Error_On_WeatherService_Failure()
        {
            // Arrange
            using var context = CreateContext();
            var taskService = new TaskService(context, _mockWeatherService.Object, _mockErrorLogger.Object);

            _mockWeatherService.Setup(s => s.GetWeatherAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Weather service error"));

            // Act
            await taskService.AddTaskAsync();

            // Assert
            _mockErrorLogger.Verify(l => l.LogError("Failed to fetch weather information", It.IsAny<HttpRequestException>()), Times.Once);
        }

        [Fact]
        public void IsValidDate_Should_Return_False_For_Invalid_Date()
        {
            // Arrange
            using var context = CreateContext();
            var taskService = new TaskService(context, _mockWeatherService.Object, _mockErrorLogger.Object);

            // Act
            var result = taskService.IsValidDate("2000-00-00", out _);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidDate_Should_Return_True_For_Valid_Date()
        {
            // Arrange
            using var context = CreateContext();
            var taskService = new TaskService(context, _mockWeatherService.Object, _mockErrorLogger.Object);

            // Act
            var result = taskService.IsValidDate("2024-07-08", out var date);

            // Assert
            Assert.True(result);
            Assert.Equal(new DateTime(2024, 7, 8), date);
        }
    }
}
