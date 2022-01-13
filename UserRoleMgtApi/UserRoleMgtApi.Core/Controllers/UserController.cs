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
using UserRoleMgtApi.Models;
using Microsoft.AspNetCore.Identity;

namespace UserRoleMgtApi.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userMgr;

        public UserController(IMapper mapper, IUserService userService, UserManager<User> useManager)
        {
            _mapper = mapper;
            _userService = userService;
            _userMgr = useManager;
        }


        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(RegisterDto model)
        {
            // if user already exist return early
            var existingEmailUser = await _userMgr.FindByEmailAsync(model.Email);
            if (existingEmailUser != null)
            {
                ModelState.AddModelError("Invalid", $"User with email: {model.Email} already exists");
                return BadRequest(Util.BuildResponse<object>(false, "User already exists!", ModelState, null));
            }

            // map data from model to user
            var user = _mapper.Map<User>(model);


            var response = await _userMgr.CreateAsync(user, model.Password);

            if (!response.Succeeded)
            {
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<string>(false, "Failed to add user!", ModelState, null));
            }

            var res = await _userMgr.AddToRoleAsync(user, "Regular");

            if (!res.Succeeded)
            {
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<string>(false, "Failed to add user role!", ModelState, null));
            }

            // Generate confirm email token here
            var token = await _userMgr.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("ConfrimEmail", "User", new { Email = user.Email, Token = token }, Request.Scheme);  // this is the url to send

            // next thing TODO here is to send an email to this new user to the email provided using a notification service you should build

            // map data to dto
            var details = _mapper.Map<RegisterSuccessDto>(user);

            // the confirmation link is added to this response object for testing purpose since at this point it is not being sent via mail
            return Ok(Util.BuildResponse(true, "New user added!", null, new { details, ConfimationLink = url }));

        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfrimEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError("Invalid", "UserId and token is required");
                return BadRequest(Util.BuildResponse<object>(false, "UserId or token is empty!", ModelState, null));
            }

            var user = await _userMgr.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("NotFound", $"User with email: {email} was not found");
                return NotFound(Util.BuildResponse<object>(false, "User not found!", ModelState, null));
            }

            var res = await _userMgr.ConfirmEmailAsync(user, token);
            if (!res.Succeeded)
            {
                foreach (var err in res.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Util.BuildResponse<object>(false, "Failed to confirm email", ModelState, null));
            }

            return Ok(Util.BuildResponse<object>(true, "Email confirmation suceeded!", null, null));
        }

        [HttpGet("get-users")]
        [AllowAnonymous]
        public IActionResult GetUsers(int page, int perPage)
        {
            // map data from db to dto to reshape it and remove null fields
            var listOfUsersToReturn = new List<UserToReturnDto>();

            //var users = _userService.Users;
            var users = _userMgr.Users.ToList();

            if (users != null)
            {
                var pagedList = PagedList<User>.Paginate(users, page, perPage);
                foreach (var user in pagedList)
                {
                    listOfUsersToReturn.Add(_mapper.Map<UserToReturnDto>(user));
                }

                var res = PagedList<UserToReturnDto>.GetPagedData(listOfUsersToReturn, page, perPage, users.Count);

                return Ok(Util.BuildResponse(true, "List of users", null, res));
            }
            else
            {
                ModelState.AddModelError("Notfound", "There was no record for users found!");
                var res = Util.BuildResponse<List<UserToReturnDto>>(false, "No results found!", ModelState, null);
                return NotFound(res);
            }

        }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser(string email)
        {
            // map data from db to dto to reshape it and remove null fields
            var UserToReturn = new UserToReturnDto();
            //var user = await _userService.GetUser(email);
            var user = await _userMgr.FindByEmailAsync(email);
            if (user != null)
            {
                UserToReturn = new UserToReturnDto
                {
                    Id = user.Id,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Email = user.Email
                };

                var res = Util.BuildResponse(true, "User details", null, UserToReturn);
                return Ok(res);
            }
            else
            {
                ModelState.AddModelError("Notfound", $"There was no record found for user with email {user.Email}");
                return NotFound(Util.BuildResponse<List<UserToReturnDto>>(false, "No result found!", ModelState, null));
            }

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
