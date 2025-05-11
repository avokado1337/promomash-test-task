using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Infrastructure.Data
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;

        public DbSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!_context.Segments.Any())
            {
                _context.Segments.AddRange(
                    new Segment { Id = Guid.Parse("76b1cb2b-1ddb-43f3-ae1b-1d3da50de800"), Name = "Сегментум Солар" },
                    new Segment { Id = Guid.Parse("41cb2a67-a758-4c45-9abe-5b5ec93c3366"), Name = "Сегментум Ультима" }
                );
                await _context.SaveChangesAsync();
            }

            if (!_context.Systems.Any())
            {
                _context.Systems.AddRange(
                    new Core.Models.System
                    {
                        Id = Guid.Parse("1457cde7-c18d-4e5c-af3b-1a2e79ccbf13"),
                        Name = "Солар",
                        SegmentId = Guid.Parse("76b1cb2b-1ddb-43f3-ae1b-1d3da50de800")
                    },
                    new Core.Models.System
                    {
                        Id = Guid.Parse("f9717d2a-cb80-4c40-849d-1b15c9c9f6a6"),
                        Name = "Ультрамар",
                        SegmentId = Guid.Parse("41cb2a67-a758-4c45-9abe-5b5ec93c3366")
                    },
                    new Core.Models.System
                    {
                        Id = Guid.Parse("22d2a732-884f-4dd4-8a9c-a6ba56b12cca"),
                        Name = "Ясан",
                        SegmentId = Guid.Parse("41cb2a67-a758-4c45-9abe-5b5ec93c3366")
                    }
                );
                await _context.SaveChangesAsync();
            }
            if (!_context.Planets.Any())
            {
                _context.Planets.AddRange(
                    new Planet
                    {
                        Id = Guid.Parse("73004ad0-52b7-4083-9503-ff36712b9174"),
                        Name = "Святая Терра",
                        SystemId = Guid.Parse("1457cde7-c18d-4e5c-af3b-1a2e79ccbf13")
                    },
                    new Planet
                    {
                        Id = Guid.Parse("a195b799-c694-4d88-bec2-69c82e9f90b8"),
                        Name = "Марс",
                        SystemId = Guid.Parse("1457cde7-c18d-4e5c-af3b-1a2e79ccbf13")
                    },
                    new Planet
                    {
                        Id = Guid.Parse("460ff609-9e24-4799-8927-9425d96cfe52"),
                        Name = "Макрагг",
                        SystemId = Guid.Parse("f9717d2a-cb80-4c40-849d-1b15c9c9f6a6")
                    },
                    new Planet
                    {
                        Id = Guid.Parse("7fa03a46-516d-4c61-9339-e4db3b729b31"),
                        Name = "Парменио",
                        SystemId = Guid.Parse("f9717d2a-cb80-4c40-849d-1b15c9c9f6a6")
                    },
                    new Planet
                    {
                        Id = Guid.Parse("ad0c7ebc-91c8-49b7-938d-6c8e08eaabf7"),
                        Name = "Мундус Планус",
                        SystemId = Guid.Parse("22d2a732-884f-4dd4-8a9c-a6ba56b12cca")
                    },
                    new Planet
                    {
                        Id = Guid.Parse("e0985155-291c-435f-870d-8b414ccca45e"),
                        Name = "Ценгей",
                        SystemId = Guid.Parse("22d2a732-884f-4dd4-8a9c-a6ba56b12cca")
                    }
                );
                await _context.SaveChangesAsync();
            }
        }
    }

}
