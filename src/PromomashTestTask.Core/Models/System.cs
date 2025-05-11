namespace PromomashTestTask.Core.Models
{
    public class System
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid SegmentId { get; set; }
        public Segment? Segment { get; set; }
        public ICollection<Planet> Planets { get; set; } = new List<Planet>();
    }
}
