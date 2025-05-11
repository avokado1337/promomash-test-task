using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PromomashTestTask.API.Controllers;
using PromomashTestTask.API.Dtos;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Services;
using Xunit;

namespace PromomashTestTask.API.Tests.Controllers
{
    public class SegmentControllerTests
    {
        private readonly Mock<ISegmentService> _mockSegmentService;
        private readonly Mock<ILogger<SegmentController>> _mockLogger;
        private readonly SegmentController _controller;

        public SegmentControllerTests()
        {
            _mockSegmentService = new Mock<ISegmentService>();
            _mockLogger = new Mock<ILogger<SegmentController>>();
            _controller = new SegmentController(_mockSegmentService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllSegmentums_ReturnsAllSegments()
        {
            // Arrange
            var testSegments = new List<Segment>
            {
                new Segment { Id = Guid.NewGuid(), Name = "Segmentum Solar" },
                new Segment { Id = Guid.NewGuid(), Name = "Segmentum Pacificus" }
            };

            _mockSegmentService.Setup(x => x.GetAllSegmentsAsync())
                .ReturnsAsync(testSegments);

            // Act
            var result = await _controller.GetAllSegmentums();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<SegmentDto>>(okResult.Value);

            // Materialize the result into a list for further assertions
            var returnValueList = returnValue.ToList();
            Assert.Equal(2, returnValueList.Count);
            Assert.Equal(testSegments[0].Name, returnValueList[0].Name);
            Assert.Equal(testSegments[1].Name, returnValueList[1].Name);
        }

        [Fact]
        public async Task GetAllSegmentums_Returns500WhenExceptionThrown()
        {
            // Arrange
            _mockSegmentService.Setup(x => x.GetAllSegmentsAsync())
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.GetAllSegmentums();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var responseDto = Assert.IsType<ResponseDto>(objectResult.Value);
            Assert.Equal("Ошибка Машинного Духа", responseDto.Message);
            Assert.Contains("Дух машины отказал в послушании", responseDto.Description);
        }

        [Fact]
        public async Task GetSystemsBySegment_ReturnsSystems_WhenSegmentExists()
        {
            // Arrange
            var segmentId = Guid.NewGuid();
            var testSystems = new List<Core.Models.System>
            {
                new Core.Models.System { Id = Guid.NewGuid(), Name = "Sol", SegmentId = segmentId },
                new Core.Models.System { Id = Guid.NewGuid(), Name = "Proxima", SegmentId = segmentId }
            };

            _mockSegmentService.Setup(x => x.GetSystemsBySegmentAsync(segmentId))
                .ReturnsAsync(testSystems);

            // Act
            var result = await _controller.GetSystemsBySegment(segmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<SystemDto>>(okResult.Value);

            // Materialize the result into a list for further assertions
            var returnValueList = returnValue.ToList();
            Assert.Equal(2, returnValueList.Count);
            Assert.Equal(testSystems[0].Name, returnValueList[0].Name);
            Assert.Equal(testSystems[1].Name, returnValueList[1].Name);
        }

        [Fact]
        public async Task GetSystemsBySegment_Returns404_WhenNoSystemsFound()
        {
            // Arrange
            var segmentId = Guid.NewGuid();
            _mockSegmentService.Setup(x => x.GetSystemsBySegmentAsync(segmentId))
                .ReturnsAsync(new List<Core.Models.System>());

            // Act
            var result = await _controller.GetSystemsBySegment(segmentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(notFoundResult.Value);
            Assert.Equal("Сегмент не найден", responseDto.Message);
        }

        [Fact]
        public async Task GetSystemsBySegment_Returns500_WhenExceptionThrown()
        {
            // Arrange
            var segmentId = Guid.NewGuid();
            _mockSegmentService.Setup(x => x.GetSystemsBySegmentAsync(segmentId))
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.GetSystemsBySegment(segmentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetPlanetsBySystem_ReturnsPlanets_WhenSystemExists()
        {
            // Arrange
            var systemId = Guid.NewGuid();
            var testPlanets = new List<Planet>
            {
                new Planet { Id = Guid.NewGuid(), Name = "Terra", SystemId = systemId },
                new Planet { Id = Guid.NewGuid(), Name = "Mars", SystemId = systemId }
            };

            _mockSegmentService.Setup(x => x.GetPlanetsBySystemAsync(systemId))
                .ReturnsAsync(testPlanets);

            // Act
            var result = await _controller.GetPlanetsBySystem(systemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PlanetDto>>(okResult.Value);

            // Materialize the result into a list for further assertions
            var returnValueList = returnValue.ToList();
            Assert.Equal(2, returnValueList.Count);
            Assert.Equal(testPlanets[0].Name, returnValueList[0].Name);
            Assert.Equal(testPlanets[1].Name, returnValueList[1].Name);
        }

        [Fact]
        public async Task GetPlanetsBySystem_Returns404_WhenNoPlanetsFound()
        {
            // Arrange
            var systemId = Guid.NewGuid();
            _mockSegmentService.Setup(x => x.GetPlanetsBySystemAsync(systemId))
                .ReturnsAsync(new List<Planet>());

            // Act
            var result = await _controller.GetPlanetsBySystem(systemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseDto = Assert.IsType<ResponseDto>(notFoundResult.Value);
            Assert.Equal("Планеты не найдены", responseDto.Message);
        }

        [Fact]
        public async Task GetPlanetsBySystem_Returns500_WhenExceptionThrown()
        {
            // Arrange
            var systemId = Guid.NewGuid();
            _mockSegmentService.Setup(x => x.GetPlanetsBySystemAsync(systemId))
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.GetPlanetsBySystem(systemId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetAllSegmentums_LogsInformation_WhenCalled()
        {
            // Arrange
            var testSegments = new List<Segment>
            {
                new Segment { Id = Guid.NewGuid(), Name = "Test Segment" }
            };

            _mockSegmentService.Setup(x => x.GetAllSegmentsAsync())
                .ReturnsAsync(testSegments);

            // Act
            await _controller.GetAllSegmentums();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Запрос всех сегментов Империума")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task GetSystemsBySegment_LogsError_WhenExceptionThrown()
        {
            // Arrange
            var segmentId = Guid.NewGuid();
            _mockSegmentService.Setup(x => x.GetSystemsBySegmentAsync(segmentId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _controller.GetSystemsBySegment(segmentId);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Ошибка при получении систем сегмента")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}