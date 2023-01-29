namespace HealthMonitoringApp.Core.Entities
{
    public partial class Pressure
    {
        public Guid Id { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; } = null!;
    }
}
