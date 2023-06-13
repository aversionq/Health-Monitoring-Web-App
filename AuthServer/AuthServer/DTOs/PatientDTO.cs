namespace AuthServer.DTOs
{
    public class PatientDTO
    {
        public string PatientId { get; set; }
        public string Username { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
