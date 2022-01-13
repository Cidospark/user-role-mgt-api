using System;
namespace UserRoleMgtApi.Models.Dtos
{
    public class RegisterSuccessDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
