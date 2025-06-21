using System.Threading.Tasks;
using AstroForm.Domain.Entities;

namespace AstroForm.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task SaveAsync(User user);
    }
}
