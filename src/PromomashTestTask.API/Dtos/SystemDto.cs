namespace PromomashTestTask.API.Dtos
{
    public class SystemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SegmentId { get; set; }
        public IList<PlanetDto>? Planets { get; set; }
    }
}
