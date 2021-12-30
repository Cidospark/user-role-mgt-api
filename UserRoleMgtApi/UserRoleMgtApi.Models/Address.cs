using System.ComponentModel.DataAnnotations;

namespace UserRoleMgtApi.Models
{
    public class Address
    {
        public string Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        // navigation props
        public User User { get; set; }
    }
}