using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data.EFCore.Repositories
{
    public class UserPhotoRepository : IUserPhotoRepository
    {
        private readonly AppDbContext _ctx;

        public UserPhotoRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> Add<T>(T entity)
        {
            _ctx.Add(entity);
            return await SaveChanges();
        }

        public async Task<bool> Delete<T>(T entity)
        {
            _ctx.Remove(entity);
            return await SaveChanges();
        }
        
        public async Task<bool> Edit<T>(T entity)
        {
            _ctx.Update(entity);
            return await SaveChanges();
        }

        public async Task<Photo> GetPhotoByPublicId(string PublicId)
        {
            return await _ctx.Photos.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.PublicId == PublicId);
        }

        public async Task<List<Photo>> GetPhotos()
        {
            return await _ctx.Photos.ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosByUserId(string UserId)
        {
            return await _ctx.Photos.Where(x => x.Id == UserId).ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
