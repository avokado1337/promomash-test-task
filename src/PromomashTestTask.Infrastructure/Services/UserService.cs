using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Repositories;
using PromomashTestTask.Core.Services;

namespace PromomashTestTask.Infrastructure.Services
{
    public class UserService : IGuardsmanService
    {
        private readonly IGuardsmanRepository _userRepository;

        public UserService(IGuardsmanRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddGuardsmanAsync(Guardsman guardsman, string password)
        {
            if (string.IsNullOrWhiteSpace(guardsman.VoxAddress))
                throw new ArgumentException("Вокс-передатчик не может быть пустым!.", nameof(guardsman.VoxAddress));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым!.", nameof(password));

            if (await GuardsmanExistsAsync(guardsman.VoxAddress))
                throw new InvalidOperationException("Гвардеец с таким уникальным адресом вокс-передатчика уже существует!.");

            await _userRepository.AddAsync(guardsman, password);
        }

        private async Task<bool> GuardsmanExistsAsync(string voxAddress)
        {
            var existingGuardsman = await _userRepository.FindByVoxAsync(voxAddress);
            return existingGuardsman != null;
        }
    }
}
