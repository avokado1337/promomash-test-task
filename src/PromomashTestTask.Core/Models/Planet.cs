namespace PromomashTestTask.Core.Models
{
    public class Planet
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid SystemId { get; set; }
        public System? System { get; set; }
    }
}
