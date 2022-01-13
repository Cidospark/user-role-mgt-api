using System;
using System.ComponentModel.DataAnnotations;

namespace UserRoleMgtApi.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
