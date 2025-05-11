using Moq;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Infrastructure.Services;
using Xunit;

namespace PromomashTestTask.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<IGuardsmanRepository> _userRepositoryMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IGuardsmanRepository>();
            _service = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task AddGuardsmanAsync_Throws_WhenVoxAddressIsEmpty()
        {
            // Arrange
            var guardsman = new Guardsman
            {
                VoxAddress = "",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddGuardsmanAsync(guardsman, "Password123"));

            //Assert
            Assert.Contains("Вокс-передатчик", ex.Message);
        }

        [Fact]
        public async Task AddGuardsmanAsync_Throws_WhenPasswordIsEmpty()
        {
            // Arrange
            var guardsman = new Guardsman
            {
                VoxAddress = "vox@example.com",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddGuardsmanAsync(guardsman, ""));

            //Assert
            Assert.Contains("Пароль", ex.Message);
        }

        [Fact]
        public async Task AddGuardsmanAsync_Throws_WhenUserAlreadyExists()
        {
            // Arrange
            var guardsman = new Guardsman
            {
                VoxAddress = "vox@example.com",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            _userRepositoryMock.Setup(r => r.FindByVoxAsync(guardsman.VoxAddress))
                .ReturnsAsync(new Guardsman() { VoxAddress = "vox@example.com" });

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddGuardsmanAsync(guardsman, "Password123"));

            //Assert
            Assert.Contains("уже существует", ex.Message);
        }

        [Fact]
        public async Task AddGuardsmanAsync_CallsRepository_WhenDataIsValid()
        {
            // Arrange
            var guardsman = new Guardsman
            {
                VoxAddress = "vox@example.com",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            _userRepositoryMock.Setup(r => r.FindByVoxAsync(guardsman.VoxAddress))
                .ReturnsAsync((Guardsman?)null);

            // Act
            await _service.AddGuardsmanAsync(guardsman, "Password123");

            // Assert
            _userRepositoryMock.Verify(r => r.AddAsync(guardsman, "Password123"), Times.Once);
        }
    }
}