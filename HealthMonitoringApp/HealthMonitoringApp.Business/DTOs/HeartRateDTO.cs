using HealthMonitoringApp.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.DTOs
{
    public class HeartRateDTO
    {
        public Guid Id { get; set; }
        public int Pulse { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; } = null!;
        public string MedicalState { get; set; }
    }
}
