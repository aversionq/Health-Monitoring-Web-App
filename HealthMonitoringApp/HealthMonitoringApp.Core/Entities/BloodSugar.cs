using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Core.Entities
{
    public partial class BloodSugar
    {
        [Key]
        public Guid Id { get; set; }
        public int SugarValue { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; } = null!;
    }
}
