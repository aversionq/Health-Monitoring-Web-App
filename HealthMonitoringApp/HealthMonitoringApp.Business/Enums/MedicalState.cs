using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Enums
{
    public class MedicalState
    {
        public enum MedicalStateType
        {
            None,
            Low,
            Normal,
            Elevated,
            High
        }
    }
}
