using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data.EFCore.Repositories
{
    public interface IAddressRepository : ICRUDRepo
    {
        Task<List<Address>> GetAddresses();
        Task<Address> GetAddress(string userId);
        Task<bool> SaveChanges();
        Task<int> RowCount();
    }
}
