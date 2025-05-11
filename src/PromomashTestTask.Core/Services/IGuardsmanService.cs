using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Core.Services
{
    public interface IGuardsmanService
    {
        Task AddGuardsmanAsync(Guardsman user, string password);
    }
}