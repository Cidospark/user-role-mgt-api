using System.Threading.Tasks;

namespace UserRoleMgtApi.Data
{
    public interface ICRUDRepo
    {
        Task<bool> Add<T>(T entity);
        Task<bool> Edit<T>(T entity);
        Task<bool> Delete<T>(T entity);
    }
}
