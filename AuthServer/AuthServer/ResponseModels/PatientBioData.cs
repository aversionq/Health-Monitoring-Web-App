namespace AuthServer.ResponseModels
{
    public class PatientBioData : BmiResponse
    {
        public int? Age { get; set; }
        public string? Gender { get; set; }
    }
}
