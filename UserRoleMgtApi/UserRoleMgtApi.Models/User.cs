using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserRoleMgtApi.Models
{
    public class User
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string LastName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string FirstName { get; set; }

        public bool IsActive { get; set; }

        // navigation props
        public List<Address> Address { get; set; }
        public List<Photo> Photos { get; set; }

        public User()
        {
            Photos = new List<Photo>();
            Address = new List<Address>();
        }
    }
}
