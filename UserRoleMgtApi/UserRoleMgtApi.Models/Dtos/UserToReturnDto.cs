using System;
namespace UserRoleMgtApi.Models.Dtos
{
    public class UserToReturnDto
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }
}
