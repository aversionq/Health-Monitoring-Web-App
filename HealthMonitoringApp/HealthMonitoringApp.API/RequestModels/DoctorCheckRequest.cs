namespace HealthMonitoringApp.API.RequestModels
{
    public class DoctorCheckRequest
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
    }
}
