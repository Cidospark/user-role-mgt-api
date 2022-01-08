using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UserRoleMgtApi.Models.Dtos
{
    public class PhotoUploadDto
    {
        [Required]
        public IFormFile Photo { get; set; }

        public string PublicId { get; set; }
        public string Url { get; set; }
    }
}
