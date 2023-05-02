namespace AuthServer.RequestModels
{
    public class DoctorRoleRequest
    {
        public string UserId { get; set; }
        public PictureUpload PassportImage { get; set; }
        public PictureUpload DiplomaImage { get; set; }
    }
}
