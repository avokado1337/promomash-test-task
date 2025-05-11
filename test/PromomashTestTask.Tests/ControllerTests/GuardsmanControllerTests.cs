using Microsoft.AspNetCore.Mvc;
using Moq;
using PromomashTestTask.API.Controllers;
using PromomashTestTask.API.Dtos;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Services;
using Xunit;

namespace PromomashTestTask.API.Tests.Controllers
{
    public class GuardsmanControllerTests
    {
        private readonly Mock<IGuardsmanService> _mockGuardsmanService;
        private readonly GuardsmanController _controller;

        public GuardsmanControllerTests()
        {
            _mockGuardsmanService = new Mock<IGuardsmanService>();
            _controller = new GuardsmanController(_mockGuardsmanService.Object);
        }

        [Fact]
        public async Task AddUser_ReturnsOk_WhenValidRequest()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "commissar@imperium.terra",
                Password = "ForTheEmperor!123",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            var expectedId = Guid.NewGuid();
            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), testDto.Password))
                .Callback<Guardsman, string>((user, _) => user.Id = expectedId);

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.Contains(expectedId.ToString(), response.Description);
            Assert.Contains("Новобранец успешно записан", response.Message);
        }

        [Fact]
        public async Task AddUser_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            var testDto = new AddGuardsmanDto(); // Invalid as required fields are missing
            _controller.ModelState.AddModelError("VoxAddress", "Required");

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.Contains("Дух машины гневается", response.Message);
            Assert.Contains("Введённые данные не соответствуют требованиям", response.Description);
        }

        [Fact]
        public async Task AddUser_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "invalid-email",
                Password = "short",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            var errorMessage = "Invalid Vox address format";
            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), testDto.Password))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.Contains(errorMessage, response.Message);
            Assert.Contains("Переданные данные содержат ошибки", response.Description);
        }

        [Fact]
        public async Task AddUser_Returns500_WhenGenericExceptionThrown()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "commissar@imperium.terra",
                Password = "ForTheEmperor!123",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            var errorMessage = "Database connection failed";
            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), testDto.Password))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "commissar@imperium.terra",
                Password = "ForTheEmperor!123",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            Guardsman capturedGuardsman = null;
            string capturedPassword = null;

            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), It.IsAny<string>()))
                .Callback<Guardsman, string>((g, p) =>
                {
                    capturedGuardsman = g;
                    capturedPassword = p;
                });

            // Act
            await _controller.AddUser(testDto);

            // Assert
            Assert.NotNull(capturedGuardsman);
            Assert.Equal(testDto.VoxAddress, capturedGuardsman.VoxAddress);
            Assert.Equal(testDto.SegmentId, capturedGuardsman.SegmentId);
            Assert.Equal(testDto.SystemId, capturedGuardsman.SystemId);
            Assert.Equal(testDto.PlanetId, capturedGuardsman.PlanetId);
            Assert.Equal(testDto.Password, capturedPassword);
        }

        [Fact]
        public async Task AddUser_ReturnsCorrectResponseStructure_OnSuccess()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "commissar@imperium.terra",
                Password = "ForTheEmperor!123",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            var expectedId = Guid.NewGuid();
            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), testDto.Password))
                .Callback<Guardsman, string>((user, _) => user.Id = expectedId);

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.Equal($"Новобранец успешно записан в ряды Имперской Гвардии!", response.Message);
            Assert.Equal($"Уникальный идентификатор новобранца: {expectedId}", response.Description);
        }

        [Fact]
        public async Task AddUser_ReturnsCorrectResponseStructure_OnArgumentException()
        {
            // Arrange
            var testDto = new AddGuardsmanDto
            {
                VoxAddress = "invalid",
                Password = "short",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            var errorMessage = "Invalid format";
            _mockGuardsmanService.Setup(x => x.AddGuardsmanAsync(It.IsAny<Guardsman>(), testDto.Password))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.AddUser(testDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequestResult.Value);
            Assert.Equal($"Ошибка рекрута: {errorMessage}. Омниссия требует исправления!", response.Message);
            Assert.Equal("Переданные данные содержат ошибки. Проверьте их и повторите попытку.", response.Description);
        }
    }
}