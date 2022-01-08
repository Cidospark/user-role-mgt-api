using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRoleMgtApi.Models.Dtos;
using UserRoleMgtApi.Services;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using UserRoleMgtApi.Helpers;

namespace UserRoleMgtApi.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto([FromForm] PhotoUploadDto model, string userId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var file = model.Photo;

            if (file.Length > 0)
            {
                var res = await _userService.AddPhotoAsync(model, userId);

                if (res.Item1)
                {
                    return Ok(Util.BuildResponse<object>(true, "Added successfully!", null, new { res.Item2.PublicId, res.Item2.Url }));
                }

                ModelState.AddModelError("Failed", "File could not be added.");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Failed to add", ModelState, null));

            }

            ModelState.AddModelError("Invalid", "File size must not be empty");
            return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "File is empty", ModelState, null));

        }

        // api/Photo/get-user-photos?userId=d274fa2f-201a-41f9-8a89-f17c4f07544d
        [HttpGet("get-user-photos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPhotos(string userId)
        {
            var photos = await _userService.GetUserPhotosAsync(userId);
            if (photos == null)
            {
                ModelState.AddModelError("Not found", "No result found for photos");
                return NotFound(Util.BuildResponse<ImageUploadResult>(false, "Result is empty", ModelState, null));
            }

            // map result
            var listOfUsersToReturn = new List<PhotoToReturnDto>();
            foreach (var photo in photos)
            {
                var photosToReturn = _mapper.Map<PhotoToReturnDto>(photo);
                listOfUsersToReturn.Add(photosToReturn);

            }

            return Ok(Util.BuildResponse<List<PhotoToReturnDto>>(true, "List of user's photos", null, listOfUsersToReturn));
        }

        // api/Photos/get-user-main-photo?userId=d274fa2f-201a-41f9-8a89-f17c4f07544d
        [HttpGet("get-user-main-photo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserMainPhoto(string userId)
        {
            var photo = await _userService.GetUserMainPhotoAsync(userId);
            if (photo == null)
            {
                ModelState.AddModelError("Not found", "No result found for main photo");
                return NotFound(Util.BuildResponse<ImageUploadResult>(false, "Result is empty", ModelState, null));
            }

            // map result
            var photosToReturn = _mapper.Map<PhotoToReturnDto>(photo);


            return Ok(Util.BuildResponse<PhotoToReturnDto>(true, "User's main photo", null, photosToReturn));
        }

        // api/Photos/set-main-photo/kbumxhsvbyqyebrtagmk?userId=d274fa2f-201a-41f9-8a89-f17c4f07544d
        [HttpPatch("set-main-photo/{publicId}")]
        public async Task<IActionResult> SetMainPhoto(string userId, string publicId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _userService.SetMainPhotoAsync(userId, publicId);
            if (!res.Item1)
            {
                ModelState.AddModelError("Failed", "Could not set main photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Set main failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Main photo is set sucessfully!", null, res.Item2));

        }

        [HttpPatch("unset-main-photo")]
        public async Task<IActionResult> UnsetMainPhoto(string userId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _userService.UnSetMainPhotoAsync(userId);
            if (!res)
            {
                ModelState.AddModelError("Failed", "Could not unset main photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Unset failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Unset main photo is sucessful!", null, ""));

        }

        // api/Photos/delete-photo/publicId=kbumxhsvbyqyebrtagmk?userId=d274fa2f-201a-41f9-8a89-f17c4f07544d
        [HttpDelete("delete-photo/{publicId}")]
        public async Task<IActionResult> DeletePhoto(string userId, string publicId)
        {
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            var res = await _userService.DeletePhotoAsync(publicId);
            if (!res)
            {
                ModelState.AddModelError("Failed", "Could not delete photo");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Delete failed!", ModelState, null));
            }

            return Ok(Util.BuildResponse<string>(true, "Photo deleted sucessful!", null, ""));

        }
    }
}
