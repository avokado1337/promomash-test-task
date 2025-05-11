using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Core.Repositories
{
    public interface IGuardsmanRepository
    {
        Task AddAsync(Guardsman user, string password);
        Task<Guardsman?> FindByVoxAsync(string voxAddress);
    }
}