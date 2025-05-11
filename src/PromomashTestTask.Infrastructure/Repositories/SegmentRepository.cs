using Microsoft.EntityFrameworkCore;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Infrastructure.Data;

namespace PromomashTestTask.Infrastructure.Repositories
{
    public class SegmentRepository : ISegmentRepository
    {
        private readonly ApplicationDbContext _context;

        public SegmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Segment>> GetAllSegmentsWithSystemsAndPlanetsAsync()
        {
            return await _context.Segments
                .AsNoTracking()
                .Include(s => s.Systems!)
                    .ThenInclude(sys => sys.Planets!)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Models.System>> GetSystemsAndPlanetsBySegmentAsync(Guid segmentId)
        {
            var segment = await _context.Segments
                .AsNoTracking()
                .Include(s => s.Systems!)
                    .ThenInclude(sys => sys.Planets!)
                .FirstOrDefaultAsync(s => s.Id == segmentId);

            return segment?.Systems ?? Enumerable.Empty<Core.Models.System>();
        }
    }
}