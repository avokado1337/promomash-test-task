namespace PromomashTestTask.API.Dtos
{
    public class SegmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IList<SystemDto>? Systems { get; set; }
    }
}
