using Microsoft.AspNetCore.Identity;
using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Infrastructure.Data
{
    public class ApplicationGuardsman : IdentityUser<Guid>
    {
        public Guid SegmentId { get; set; }
        public Guid SystemId { get; set; }
        public Guid PlanetId { get; set; }
        public virtual Segment? Country { get; set; }
        public virtual Core.Models.System? Province { get; set; }
        public virtual Planet? Planet { get; set; }

    }
}