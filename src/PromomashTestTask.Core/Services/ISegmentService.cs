using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Core.Services
{
    public interface ISegmentService
    {
        Task<IEnumerable<Segment>> GetAllSegmentsAsync();
        Task<IEnumerable<Core.Models.System>> GetSystemsBySegmentAsync(Guid segmentId);
        Task<IEnumerable<Planet>> GetPlanetsBySystemAsync(Guid systemId);
    }
}
