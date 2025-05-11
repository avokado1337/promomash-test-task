namespace PromomashTestTask.API.Dtos
{
    public class PlanetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SystemId { get; set; }
    }
}