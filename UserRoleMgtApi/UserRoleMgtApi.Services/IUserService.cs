using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Services
{
    public interface IUserService
    {
        public Task<Tuple<bool, PhotoUploadDto>> AddPhotoAsync(PhotoUploadDto model, string userId);
        public Task<List<Photo>> GetUserPhotosAsync(string userId);
        public Task<Photo> GetUserMainPhotoAsync(string userId);
        public Task<Tuple<bool, string>> SetMainPhotoAsync(string userId, string PublicId);
        public Task<bool> UnSetMainPhotoAsync(string userId);
        public Task<bool> DeletePhotoAsync(string PublicId);
    }
}
