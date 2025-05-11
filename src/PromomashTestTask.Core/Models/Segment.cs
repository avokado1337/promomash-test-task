namespace PromomashTestTask.Core.Models
{
    public class Segment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<System> Systems { get; set; } = new List<System>();
    }
}