using System;

namespace UserRoleMgtApi.Models
{
    public class Photo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsMain { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        // navigation props
        public string UserId { get; set; }
        public User AppUser { get; set; }
    }
}