using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Infrastructure.Data;
using PromomashTestTask.Infrastructure.Repositories;

namespace PromomashTestTask.Tests.RepositoryTests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repository;
        private readonly Mock<UserManager<ApplicationGuardsman>> _userManagerMock;
        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var store = new Mock<IUserStore<ApplicationGuardsman>>();
            _userManagerMock = new Mock<UserManager<ApplicationGuardsman>>(
                store.Object, null, null, null, null, null, null, null, null);
            _repository = new UserRepository(_context, _userManagerMock.Object);
        }

        [Fact]
        public async Task AddAsync_Throws_When_Segment_Not_Exists()
        {
            var user = new Guardsman
            {
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid(),
                VoxAddress = "test@vox.com"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.AddAsync(user, "123"));
            Assert.Contains("Сегментум", ex.Message);
        }

        [Fact]
        public async Task AddAsync_Creates_User_When_Valid()
        {
            var segmentId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var planetId = Guid.NewGuid();

            _context.Segments.Add(new Segment { Id = segmentId, Name = "Some segment" });
            _context.Systems.Add(new PromomashTestTask.Core.Models.System
            {
                Id = systemId,
                SegmentId = segmentId,
                Name = "Some system"
            });
            _context.Planets.Add(new Planet
            {
                Id = planetId,
                SystemId = systemId,
                Name = "Some planet"
            });
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationGuardsman>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var repo = new UserRepository(_context, _userManagerMock.Object);

            var user = new Guardsman
            {
                SegmentId = segmentId,
                SystemId = systemId,
                PlanetId = planetId,
                VoxAddress = "test@vox.com"
            };

            await repo.AddAsync(user, "password");

            _userManagerMock.Verify(m => m.CreateAsync(It.Is<ApplicationGuardsman>(u =>
                u.Email == "test@vox.com" &&
                u.SegmentId == segmentId &&
                u.SystemId == systemId &&
                u.PlanetId == planetId
            ), "password"), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Throws_When_System_Does_Not_Belong_To_Segment()
        {
            var segmentId = Guid.NewGuid();
            var wrongSegmentId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var planetId = Guid.NewGuid();

            _context.Segments.Add(new Segment { Id = segmentId, Name = "Some segment" });
            _context.Segments.Add(new Segment { Id = wrongSegmentId, Name = "Wrong segment" });
            _context.Systems.Add(new PromomashTestTask.Core.Models.System
            {
                Id = systemId,
                SegmentId = wrongSegmentId,
                Name = "Some system"
            });
            _context.Planets.Add(new Planet
            {
                Id = planetId,
                SystemId = systemId,
                Name = "Some planet"
            });

            await _context.SaveChangesAsync();

            var repo = new UserRepository(_context, _userManagerMock.Object);
            var user = new Guardsman
            {
                SegmentId = segmentId,
                SystemId = systemId,
                PlanetId = planetId,
                VoxAddress = "test@vox.com"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddAsync(user, "pass"));
            Assert.Contains("не принадлежит сегментуму", ex.Message);
        }

        [Fact]
        public async Task FindByVoxAsync_Returns_Guardsman_When_Exists()
        {
            var userId = Guid.NewGuid();
            var identity = new ApplicationGuardsman
            {
                Id = userId,
                Email = "vox@vox.com",
                SegmentId = Guid.NewGuid(),
                SystemId = Guid.NewGuid(),
                PlanetId = Guid.NewGuid()
            };

            _context.Users.Add(identity);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(m => m.FindByEmailAsync("vox@vox.com"))
                .ReturnsAsync(identity);

            var repo = new UserRepository(_context, _userManagerMock.Object);
            var result = await repo.FindByVoxAsync("vox@vox.com");
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByVoxAsync_Returns_Null_When_User_Not_Found()
        {
            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationGuardsman?)null);

            var repo = new UserRepository(_context, _userManagerMock.Object);
            var result = await repo.FindByVoxAsync("notfound@vox.com");

            Assert.Null(result);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}