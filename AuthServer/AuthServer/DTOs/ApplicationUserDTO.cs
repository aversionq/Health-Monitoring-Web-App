using AuthServer.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.DTOs
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime RegistrationDate { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
