using System.ComponentModel.DataAnnotations;

namespace AuthServer.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [StringLength(70, ErrorMessage = "Max length of first name is 70 characters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(70, ErrorMessage = "Max length of last name is 70 characters")]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Username or Email is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
