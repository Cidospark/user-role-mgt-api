using System;
using Microsoft.AspNetCore.Identity;
using UserRoleMgtApi.Models;

namespace UserRoleMgtApi.Services
{
    public class PhotoService
    {
        private readonly UserManager<User> _userMgr;
        //private readonly IPhotoRepository _photoRepo;

        public PhotoService()
        {
        }
    }
}
