using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserRoleMgtApi.Data.EFCore.Repositories;
using UserRoleMgtApi.Helpers;
using UserRoleMgtApi.Models;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config,
            IMapper mapper, UserManager<User> userManager)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<Tuple<bool, PhotoUploadDto>> UploadPhotoAsync(PhotoUploadDto model)
        {
            var uploadResult = new ImageUploadResult();

            using (var stream = model.Photo.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.Photo.FileName, stream),
                    Transformation = new Transformation().Width(300).Height(300).Gravity("face").Crop("fill")
                };

                uploadResult = await _cloudinary.UploadAsync(imageUploadParams);
            }

            var status = uploadResult.StatusCode.ToString();

            if (status.Equals("OK"))
            {
                model.PublicId = uploadResult.PublicId;
                model.Url = uploadResult.Url.ToString();
                return new Tuple<bool, PhotoUploadDto>(true, model);
            }

            return new Tuple<bool, PhotoUploadDto>(false, model);

        }

        public async Task<bool> DeletePhotoAsync(string PublicId)
        {
            DeletionParams destroyParams = new DeletionParams(PublicId)
            {
                ResourceType = ResourceType.Image
            };

            DeletionResult destroyResult = await _cloudinary.DestroyAsync(destroyParams);

            if (destroyResult.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }

    }
}
