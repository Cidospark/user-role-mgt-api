using System;
using Microsoft.AspNetCore.Identity;
using UserRoleMgtApi.Data.EFCore.Repositories;
using UserRoleMgtApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleMgtApi.Models.Dtos;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UserRoleMgtApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userMgr;
        private readonly IUserPhotoRepository _photoRepo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserService(UserManager<User> userManager, IUserPhotoRepository userPhotoRepository,
            IMapper mapper, IPhotoService photoService)
        {
            _userMgr = userManager;
            _photoRepo = userPhotoRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<Tuple<bool, PhotoUploadDto>> AddPhotoAsync(PhotoUploadDto model, string userId)
        {
            bool res = false;
            if (_photoService.UploadPhotoAsync(model).Result.Item1)
            {
                var user = await _userMgr.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == userId);

                var photo = _mapper.Map<Photo>(model);
                photo.Id = userId;

                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;

                // add photo to database
                res = await _photoRepo.Add(photo);
            }

            return new Tuple<bool, PhotoUploadDto>(res, model);
        }

        public async Task<List<Photo>> GetUserPhotosAsync(string userId)
        {
            var res = await _photoRepo.GetPhotosByUserId(userId);
            if (res != null)
                return res;

            return null;
        }

        public async Task<Photo> GetUserMainPhotoAsync(string userId)
        {
            var res = await _photoRepo.GetPhotosByUserId(userId);

            var mainPhoto = res.FirstOrDefault(x => x.IsMain == true);

            if (mainPhoto != null)
                return mainPhoto;

            return null;
        }

        public async Task<Tuple<bool, string>> SetMainPhotoAsync(string userId, string PublicId)
        {
            var photos = await _photoRepo.GetPhotosByUserId(userId);
            if (photos != null)
            {
                this.UnsetMain(photos);

                var newMain = photos.FirstOrDefault(x => x.PublicId == PublicId);
                newMain.IsMain = true;

                // update database
                var res = await _photoRepo.Edit(newMain);
                if (res)
                    return new Tuple<bool, string>(true, newMain.Url);
            }

            return new Tuple<bool, string>(false, "");
        }

        public async Task<bool> UnSetMainPhotoAsync(string userId)
        {
            var photos = await _photoRepo.GetPhotosByUserId(userId);
            if (photos != null)
            {
                this.UnsetMain(photos);

                // update database
                var res = await _photoRepo.SaveChanges();
                if (res)
                    return true;
            }

            return false;
        }

        private void UnsetMain(List<Photo> photos)
        {
            if (photos.Any(x => x.IsMain))
            {
                // get the main photo and unset it
                var main = photos.FirstOrDefault(x => x.IsMain == true);
                main.IsMain = false;
            }
        }

        public async Task<bool> DeletePhotoAsync(string PublicId)
        {
            bool res = await _photoService.DeletePhotoAsync(PublicId);

            if (res)
            {
                var photo = await _photoRepo.GetPhotoByPublicId(PublicId);
                if (photo != null)
                {
                    var res2 = await _photoRepo.Delete(photo);
                    if (res2)
                        return true;
                }
            }

            return false;
        }
    }
}
