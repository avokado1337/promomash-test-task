using Moq;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Infrastructure.Services;
using Xunit;

namespace PromomashTestTask.Tests.ServiceTests
{
    public class SegmentServiceTests
    {
        private readonly Mock<ISegmentRepository> _segmentRepositoryMock;
        private readonly SegmentService _service;

        public SegmentServiceTests()
        {
            _segmentRepositoryMock = new Mock<ISegmentRepository>();
            _service = new SegmentService(_segmentRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllSegmentsAsync_ReturnsSegmentsWithoutSystems()
        {
            // Arrange
            var segments = new List<Segment>
            {
                new Segment { Id = Guid.NewGuid(), Name = "Segment 1", Systems = new List<Core.Models.System>() },
                new Segment { Id = Guid.NewGuid(), Name = "Segment 2", Systems = new List<Core.Models.System>() }
            };

            _segmentRepositoryMock
                .Setup(repo => repo.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .ReturnsAsync(segments);

            // Act
            var result = await _service.GetAllSegmentsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, s => Assert.NotNull(s.Id));
            Assert.All(result, s => Assert.False(string.IsNullOrWhiteSpace(s.Name)));
        }

        [Fact]
        public async Task GetAllSegmentsAsync_ReturnsEmpty_WhenNoSegmentsExist()
        {
            // Arrange
            _segmentRepositoryMock
                .Setup(repo => repo.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .ReturnsAsync(new List<Segment>());

            // Act
            var result = await _service.GetAllSegmentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSystemsBySegmentAsync_ReturnsSystemsWithoutPlanets()
        {
            // Arrange
            var systems = new List<Core.Models.System>
            {
                new Core.Models.System { Id = Guid.NewGuid(), Name = "System 1", SegmentId = Guid.NewGuid() },
                new Core.Models.System { Id = Guid.NewGuid(), Name = "System 2", SegmentId = Guid.NewGuid() }
            };

            _segmentRepositoryMock
                .Setup(repo => repo.GetSystemsAndPlanetsBySegmentAsync(It.IsAny<Guid>()))
                .ReturnsAsync(systems);

            // Act
            var result = await _service.GetSystemsBySegmentAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, s => Assert.NotNull(s.Id));
            Assert.All(result, s => Assert.False(string.IsNullOrWhiteSpace(s.Name)));
        }

        [Fact]
        public async Task GetSystemsBySegmentAsync_ReturnsEmpty_WhenNoSystems()
        {
            // Arrange
            _segmentRepositoryMock
                .Setup(repo => repo.GetSystemsAndPlanetsBySegmentAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Core.Models.System>());

            // Act
            var result = await _service.GetSystemsBySegmentAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPlanetsBySystemAsync_ReturnsPlanets_WhenSystemExists()
        {
            // Arrange
            var systemId = Guid.NewGuid();
            var planet = new Planet { Id = Guid.NewGuid(), Name = "Planet 1" };

            var systems = new List<Core.Models.System>
            {
                new Core.Models.System { Id = systemId, Name = "System 1", Planets = new List<Planet> { planet } }
            };

            var segments = new List<Segment>
            {
                new Segment { Id = Guid.NewGuid(), Name = "Segment 1", Systems = systems }
            };

            _segmentRepositoryMock
                .Setup(repo => repo.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .ReturnsAsync(segments);

            // Act
            var result = await _service.GetPlanetsBySystemAsync(systemId);

            // Assert
            Assert.Single(result);
            Assert.Equal(planet.Id, result.First().Id);
        }

        [Fact]
        public async Task GetPlanetsBySystemAsync_ReturnsEmpty_WhenSystemDoesNotExist()
        {
            // Arrange
            var segments = new List<Segment>
            {
                new Segment
                {
                    Id = Guid.NewGuid(),
                    Name = "Segment 1",
                    Systems = new List<Core.Models.System>
                    {
                        new Core.Models.System
                        {
                            Id = Guid.NewGuid(), // Не тот systemId
                            Name = "System X",
                            Planets = new List<Planet>
                            {
                                new Planet { Id = Guid.NewGuid(), Name = "Planet X" }
                            }
                        }
                    }
                }
            };

            _segmentRepositoryMock
                .Setup(repo => repo.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .ReturnsAsync(segments);

            // Act
            var result = await _service.GetPlanetsBySystemAsync(Guid.NewGuid()); 

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPlanetsBySystemAsync_ReturnsEmpty_WhenSegmentIsNull()
        {
            // Arrange
            _segmentRepositoryMock
                .Setup(repo => repo.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .ReturnsAsync(new List<Segment>()); 

            // Act
            var result = await _service.GetPlanetsBySystemAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
