using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data.EFCore.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _ctx;

        public AddressRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<bool> Add<T>(T entity)
        {
            _ctx.Add(entity);
            return SaveChanges();
        }

        public Task<bool> Delete<T>(T entity)
        {
            _ctx.Remove(entity);
            return SaveChanges();
        }

        public Task<bool> Edit<T>(T entity)
        {
            _ctx.Update(entity);
            return SaveChanges();
        }

        public async Task<Address> GetAddress(string userId)
        {
            return await _ctx.Addresses.Include(x => x.User).FirstAsync(x => x.Id == userId);
        }

        public async Task<List<Address>> GetAddresses()
        {
            return await _ctx.Addresses.ToListAsync();
        }

        public async Task<int> RowCount()
        {
            return await _ctx.Addresses.CountAsync(); ;
        }

        public async Task<bool> SaveChanges()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
