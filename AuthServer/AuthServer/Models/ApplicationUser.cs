using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(70, ErrorMessage = "Max length of first name is 70 characters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(70, ErrorMessage = "Max length of last name is 70 characters")]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }

    }
}
