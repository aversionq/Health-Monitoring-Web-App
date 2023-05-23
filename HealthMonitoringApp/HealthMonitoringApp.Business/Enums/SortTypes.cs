using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Enums
{
    public class SortTypes
    {
        public enum PressureSort
        {
            DateAsc,
            DateDesc,
            SystolicDesc,
            SystolicAsc,
            DiastolicDesc,
            DiastolicAsc
        }

        public enum BloodSugarSort
        {
            DateAsc,
            DateDesc,
            SugarDesc,
            SugarAsc
        }

        public enum HeartRateSort
        {
            DateAsc,
            DateDesc,
            PulseDesc,
            PulseAsc
        }
    }
}
