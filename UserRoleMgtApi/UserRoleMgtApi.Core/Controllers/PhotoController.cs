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
    public class PhotoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public PhotoController(IMapper mapper, IPhotoService photoService)
        {
            _mapper = mapper;
            _photoService = photoService;
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
                var uploadStatus = await _photoService.UploadPhotoAsync(model, userId);

                if (uploadStatus.Item1)
                {
                    var res = await _photoService.AddPhotoAsync(model, userId);
                    if (!res.Item1)
                    {
                        ModelState.AddModelError("Failed", "Could not add photo to database");
                        return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Failed to add to database", ModelState, null));
                    }

                    return Ok(Util.BuildResponse<object>(true, "Uploaded successfully", null, new { res.Item2.PublicId, res.Item2.Url }));
                }

                ModelState.AddModelError("Failed", "File could not be uploaded to cloudinary");
                return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "Failed to upload", ModelState, null));

            }

            ModelState.AddModelError("Invalid", "File size must not be empty");
            return BadRequest(Util.BuildResponse<ImageUploadResult>(false, "File is empty", ModelState, null));

        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
