using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Api;
using BrainstormSessions.Controllers;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit;

namespace BrainstormSessions.Test.UnitTests
{
    public class LoggingTests : IDisposable
    {
        public LoggingTests()
        {
            // Configure Serilog to use InMemory sink for testing
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.InMemory()
                .CreateLogger();
        }

        public void Dispose()
        {
            // Clear in-memory logs after each test
            InMemorySink.Instance.Dispose();
            Log.CloseAndFlush();
        }

        [Fact]
        public async Task HomeController_Index_LogInfoMessages()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(GetTestSessions());
            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            var logger = loggerFactory.CreateLogger<HomeController>();
            
            var controller = new HomeController(mockRepo.Object, logger);

            // Act
            var result = await controller.Index();

            // Assert
            var logEvents = InMemorySink.Instance.LogEvents;
            Assert.True(logEvents.Any(l => l.Level == LogEventLevel.Information), 
                "Expected Info messages in the logs");
        }

        [Fact]
        public async Task HomeController_IndexPost_LogWarningMessage_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(GetTestSessions());
            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            var logger = loggerFactory.CreateLogger<HomeController>();
            
            var controller = new HomeController(mockRepo.Object, logger);
            controller.ModelState.AddModelError("SessionName", "Required");
            var newSession = new HomeController.NewSessionModel();

            // Act
            var result = await controller.Index(newSession);

            // Assert
            var logEvents = InMemorySink.Instance.LogEvents;
            Assert.True(logEvents.Any(l => l.Level == LogEventLevel.Warning), 
                "Expected Warn messages in the logs");
        }

        [Fact]
        public async Task IdeasController_CreateActionResult_LogErrorMessage_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            var logger = loggerFactory.CreateLogger<IdeasController>();
            
            var controller = new IdeasController(mockRepo.Object, logger);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.CreateActionResult(model: null);

            // Assert
            var logEvents = InMemorySink.Instance.LogEvents;
            Assert.True(logEvents.Any(l => l.Level == LogEventLevel.Error), 
                "Expected Error messages in the logs");
        }

        [Fact]
        public async Task SessionController_Index_LogDebugMessages()
        {
            // Arrange
            int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(GetTestSessions().FirstOrDefault(
                    s => s.Id == testSessionId));
            
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });
            var logger = loggerFactory.CreateLogger<SessionController>();
            
            var controller = new SessionController(mockRepo.Object, logger);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            var logEvents = InMemorySink.Instance.LogEvents;
            Assert.True(logEvents.Count(l => l.Level == LogEventLevel.Debug) == 2, 
                "Expected 2 Debug messages in the logs");
        }

        private List<BrainstormSession> GetTestSessions()
        {
            var sessions = new List<BrainstormSession>
            {
                new BrainstormSession()
                {
                    DateCreated = new DateTime(2016, 7, 2),
                    Id = 1,
                    Name = "Test One"
                },
                new BrainstormSession()
                {
                    DateCreated = new DateTime(2016, 7, 1),
                    Id = 2,
                    Name = "Test Two"
                }
            };
            return sessions;
        }
    }
}
