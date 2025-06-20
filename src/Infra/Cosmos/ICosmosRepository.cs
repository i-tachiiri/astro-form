using System.Collections.Generic;
using System.Threading.Tasks;
namespace AstroForm.Infra.Cosmos;

public interface ICosmosRepository<T>
{
    Task<T?> GetAsync(string id, string partitionKey);
    Task<IEnumerable<T>> QueryAsync(string query);
    Task SaveAsync(T entity);
    Task DeleteAsync(string id, string partitionKey);
}
