namespace AuthServer.DTOs
{
    public class DoctorDTO
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string Username { get; set; }
        public string? Gender { get; set; }
        public bool IsContactedWithCurrentUser { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
