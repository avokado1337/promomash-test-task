using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Core.Services;
using PromomashTestTask.Infrastructure.Data;
using PromomashTestTask.Infrastructure.Repositories;
using PromomashTestTask.Infrastructure.Services;

namespace PromomashTestTask.API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationGuardsman, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.AddScoped<IGuardsmanRepository, UserRepository>();
            builder.Services.AddScoped<IGuardsmanService, UserService>();
            builder.Services.AddScoped<ISegmentRepository, SegmentRepository>();
            builder.Services.AddScoped<ISegmentService, SegmentService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DockerComposePolicy", policy => policy
                    .WithOrigins(
                        "http://localhost:4200",
                        "http://web:4200",
                        "http://localhost:80",
                        "http://web:80"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<DbSeeder>();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await dbContext.Database.MigrateAsync();
                await seeder.SeedAsync();
            }
            app.Run();
        }
    }
}
