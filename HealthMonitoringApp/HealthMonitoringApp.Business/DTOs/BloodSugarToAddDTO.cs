using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.DTOs
{
    public class BloodSugarToAddDTO
    {
        public int SugarValue { get; set; }
        public DateTime Date { get; set; }
    }
}
