using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoringApp.Core.Entities;

namespace HealthMonitoringApp.Application.Interfaces
{
    public interface IPressureRepository
    {
        public Task<Pressure> GetPressureById(Guid pressureId);
        public Task<IEnumerable<Pressure>> GetUserPressure(string userId);
        public Task AddPressure(Pressure pressure);
        public Task UpdatePressure(Pressure updPressure);
        public Task DeletePressure(Pressure pressure);
    }
}
