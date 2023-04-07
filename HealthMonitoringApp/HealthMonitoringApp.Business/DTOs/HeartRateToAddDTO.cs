using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.DTOs
{
    public class HeartRateToAddDTO
    {
        public int Pulse { get; set; }
        public DateTime Date { get; set; }
    }
}
