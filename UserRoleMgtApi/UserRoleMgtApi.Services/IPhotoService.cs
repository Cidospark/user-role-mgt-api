using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Services
{
    public interface IPhotoService
    {
        public Task<Tuple<bool, PhotoUploadDto>> UploadPhotoAsync(PhotoUploadDto model);
        public Task<bool> DeletePhotoAsync(string PublicId);
    }
}
