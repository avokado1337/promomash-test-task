using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Infrastructure.Data;

namespace PromomashTestTask.Infrastructure.Repositories
{
    public class UserRepository : IGuardsmanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationGuardsman> _userManager;
        public UserRepository(ApplicationDbContext context, UserManager<ApplicationGuardsman> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task AddAsync(Guardsman user, string password)
        {
            var segmentExists = await _context.Segments.AnyAsync(c => c.Id == user.SegmentId);
            if (!segmentExists)
            {
                throw new InvalidOperationException($"Сегментум с идентификатором {user.SegmentId} не существует!.");
            }

            var systemExists = await _context.Systems.AnyAsync(p => p.Id == user.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"Система с идентификатором {user.SystemId} не существует!.");
            }

            var planetExists = await _context.Planets.AnyAsync(p => p.Id == user.PlanetId);
            if (!planetExists)
            {
                throw new InvalidOperationException($"Планеты с идентификатором {user.PlanetId} не существует!.");
            }
            var systemBelongsToSegment = await _context.Systems
                .AnyAsync(p => p.Id == user.SystemId && p.SegmentId == user.SegmentId);
            if (!systemBelongsToSegment)
            {
                throw new InvalidOperationException
                    ($"Система с идентификатором {user.SystemId} не принадлежит сегментуму с идентификатором {user.SegmentId}!.");
            }


            var planetBelongsToSystem = await _context.Planets
                .AnyAsync(p => p.Id == user.PlanetId && p.SystemId == user.SystemId);
            if (!planetBelongsToSystem)
            {
                throw new InvalidOperationException
                    ($"Планета с идентификатором {user.PlanetId} не принадлежит системе с идентификатором {user.SystemId}!.");
            }

            var identity = new ApplicationGuardsman
            {
                UserName = user.VoxAddress,
                Email = user.VoxAddress,
                SegmentId = user.SegmentId,
                SystemId = user.SystemId,
                PlanetId = user.PlanetId,
            };

            var result = await _userManager.CreateAsync(identity, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Ошибка при добавлении новобранца: {errors}");
            }
        }

        public async Task<Guardsman?> FindByVoxAsync(string voxAddress)
        {
            var identity = await _userManager.FindByEmailAsync(voxAddress);
            if (identity == null) return null;

            var appUser = await _context.Users
                .Include(u => u.Country)
                .Include(u => u.Province)
                .FirstOrDefaultAsync(u => u.Id == identity.Id);

            if (appUser == null) return null;

            return new Guardsman
            {
                Id = appUser.Id,
                VoxAddress = appUser.Email,
                SegmentId = appUser.SegmentId,
                SystemId = appUser.SystemId,
                PlanetId = appUser.PlanetId
            };
        }
    }
}
