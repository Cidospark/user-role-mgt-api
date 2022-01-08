using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Data.EFCore.Repositories
{
    public interface IPhotoRepository : ICRUDRepo
    {
        Task<bool> SaveChanges();
        Task<List<Photo>> GetPhotos();
        Task<Photo> GetPhotoByPublicId(string PublicId);
        Task<List<Photo>> GetPhotosByUserId(string UserId);

    }
}
