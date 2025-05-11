using Microsoft.EntityFrameworkCore;
using Moq;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Infrastructure.Data;
using PromomashTestTask.Infrastructure.Repositories;
using PromomashTestTask.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromomashTestTask.Tests.RepositoryTests
{
    public class SegmentRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly SegmentRepository _repository;

        public SegmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new SegmentRepository(_context);
        }

        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_ReturnsSegmentsWithSystemsAndPlanets()
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
                                Id = Guid.NewGuid(),
                                Name = "System 1",
                                Planets = new List<Planet>
                                {
                                    new Planet { Id = Guid.NewGuid(), Name = "Planet 1" }
                                }
                            }
                        }
                    }
                };

            _context.Segments.AddRange(segments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Single(result.First().Systems);
            Assert.Single(result.First().Systems.First().Planets);
        }

        [Fact]
        public async Task GetSystemsAndPlanetsBySegmentAsync_ReturnsSystemsAndPlanetsForGivenSegment()
        {
            // Arrange
            var segmentId = Guid.NewGuid();
            var segments = new List<Segment>
                {
                    new Segment
                    {
                        Id = segmentId,
                        Name = "Segment 1",
                        Systems = new List<Core.Models.System>
                        {
                            new Core.Models.System
                            {
                                Id = Guid.NewGuid(),
                                Name = "System 1",
                                Planets = new List<Planet>
                                {
                                    new Planet { Id = Guid.NewGuid(), Name = "Planet 1" }
                                }
                            }
                        }
                    }
                };

            _context.Segments.AddRange(segments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSystemsAndPlanetsBySegmentAsync(segmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Single(result.First().Planets);
        }

        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_ReturnsEmptyList_WhenNoSegmentsExist()
        {
            // Act
            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSystemsAndPlanetsBySegmentAsync_ReturnsEmptyList_WhenSegmentDoesNotExist()
        {
            // Act
            var result = await _repository.GetSystemsAndPlanetsBySegmentAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_ReturnsMultipleSegments()
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
                            Id = Guid.NewGuid(),
                            Name = "System A",
                            Planets = new List<Planet>
                            {
                                new Planet { Id = Guid.NewGuid(), Name = "Planet A1" }
                            }
                        }
                    }
                },
                new Segment
                {
                    Id = Guid.NewGuid(),
                    Name = "Segment 2",
                    Systems = new List<Core.Models.System>
                    {
                        new Core.Models.System
                        {
                            Id = Guid.NewGuid(),
                            Name = "System B",
                            Planets = new List<Planet>
                            {
                                new Planet { Id = Guid.NewGuid(), Name = "Planet B1" }
                            }
                        }
                    }
                }
            };

            _context.Segments.AddRange(segments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_SegmentWithoutSystems_ReturnsSegmentWithEmptySystems()
        {
            // Arrange
            var segment = new Segment
            {
                Id = Guid.NewGuid(),
                Name = "Segment No Systems",
                Systems = new List<Core.Models.System>()
            };

            _context.Segments.Add(segment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            // Assert
            Assert.Single(result);
            Assert.Empty(result.First().Systems);
        }

        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_SystemWithoutPlanets_ReturnsSystemWithEmptyPlanets()
        {
            var segment = new Segment
            {
                Id = Guid.NewGuid(),
                Name = "Segment With Empty System",
                Systems = new List<Core.Models.System>
                {
                    new Core.Models.System
                    {
                        Id = Guid.NewGuid(),
                        Name = "System Without Planets",
                        Planets = new List<Planet>()
                    }
                }
            };

            _context.Segments.Add(segment);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            Assert.Single(result);
            var system = result.First().Systems.First();
            Assert.Empty(system.Planets);
        }
        [Fact]
        public async Task GetAllSegmentsWithSystemsAndPlanetsAsync_SegmentWithNullSystems_ReturnsEmptySystems()
        {
            var segment = new Segment
            {
                Id = Guid.NewGuid(),
                Name = "Segment With Null Systems",
                Systems = null!
            };

            _context.Segments.Add(segment);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllSegmentsWithSystemsAndPlanetsAsync();

            Assert.Single(result);
            Assert.Empty(result.First().Systems); // если `null`, а не пустой список
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
