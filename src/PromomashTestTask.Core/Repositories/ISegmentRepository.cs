using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Core.Repositories
{
    public interface ISegmentRepository
    {
        Task<IEnumerable<Segment>> GetAllSegmentsWithSystemsAndPlanetsAsync();
        Task<IEnumerable<Core.Models.System>> GetSystemsAndPlanetsBySegmentAsync(Guid segmentId);
    }
}
