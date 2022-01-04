using System;
using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserRoleMgtApi.Data.EFCore.Repositories;
using UserRoleMgtApi.Helpers;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Services
{
    public class PhotoService
    {
        private readonly UserManager<User> _userMgr;
        private readonly IPhotoRepository _photoRepo;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotoService(IOptions<CloudinarySettings> config,
            IMapper mapper, UserManager<User> userManager,
            IPhotoRepository photoRepository)
        {
        }
    }
}
