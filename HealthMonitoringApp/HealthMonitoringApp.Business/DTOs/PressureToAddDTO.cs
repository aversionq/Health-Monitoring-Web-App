using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.DTOs
{
    public class PressureToAddDTO
    {
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public DateTime Date { get; set; }
    }
}
