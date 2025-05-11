using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Core.Services;

namespace PromomashTestTask.Infrastructure.Services
{
    public class SegmentService : ISegmentService
    {
        private readonly ISegmentRepository _segmentRepository;

        public SegmentService(ISegmentRepository segmentRepository)
        {
            _segmentRepository = segmentRepository;
        }

        public async Task<IEnumerable<Segment>> GetAllSegmentsAsync()
        {
            var segments = await _segmentRepository.GetAllSegmentsWithSystemsAndPlanetsAsync();
            return segments.Select(s => new Segment
            {
                Id = s.Id,
                Name = s.Name
            });
        }

        public async Task<IEnumerable<Core.Models.System>> GetSystemsBySegmentAsync(Guid segmentId)
        {
            var systems = await _segmentRepository.GetSystemsAndPlanetsBySegmentAsync(segmentId);
            return systems.Select(s => new Core.Models.System
            {
                Id = s.Id,
                Name = s.Name,
                SegmentId = s.SegmentId
            });
        }

        public async Task<IEnumerable<Planet>> GetPlanetsBySystemAsync(Guid systemId)
        {
            var segment = (await _segmentRepository.GetAllSegmentsWithSystemsAndPlanetsAsync())
                .FirstOrDefault(s => s.Systems.Any(sys => sys.Id == systemId));

            if (segment == null)
                return Enumerable.Empty<Planet>();

            var system = segment.Systems.FirstOrDefault(s => s.Id == systemId);
            return system?.Planets ?? Enumerable.Empty<Planet>();
        }
    }
}