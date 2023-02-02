using System.ComponentModel.DataAnnotations;

namespace HealthMonitoringApp.Core.Entities
{
    public partial class Pressure
    {
        [Key]
        public Guid Id { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int Pulse { get; set; }
        public DateTime Date { get; set; }
        [StringLength(450)]
        public string UserId { get; set; } = null!;
    }
}
