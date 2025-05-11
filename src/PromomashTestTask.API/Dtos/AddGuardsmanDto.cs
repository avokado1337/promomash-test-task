using System.ComponentModel.DataAnnotations;

namespace PromomashTestTask.API.Dtos
{
    public class AddGuardsmanDto
    {
        [Required]
        [EmailAddress]
        public string VoxAddress { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public Guid SegmentId { get; set; }

        [Required]
        public Guid SystemId { get; set; }

        [Required]
        public Guid PlanetId { get; set; }
    }
}