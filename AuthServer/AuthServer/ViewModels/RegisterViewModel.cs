using AuthServer.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(70, ErrorMessage = "Max length of first name is 70 characters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(70, ErrorMessage = "Max length of last name is 70 characters")]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public double Height { get; set; }
        [Required]
        public GenderType.GenderTypes Gender { get; set; }
    }
}
