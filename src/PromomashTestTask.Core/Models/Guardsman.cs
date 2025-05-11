namespace PromomashTestTask.Core.Models
{
    public class Guardsman
    {
        public Guid Id { get; set; }
        public required string VoxAddress { get; set; }
        public Guid SegmentId { get; set; }
        public Guid SystemId { get; set; }
        public Guid PlanetId { get; set; }
    }
}