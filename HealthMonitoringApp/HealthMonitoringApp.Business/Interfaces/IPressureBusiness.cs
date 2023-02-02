using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Interfaces
{
    public interface IPressureBusiness
    {
        public Task<PressureDTO> GetPressureById(Guid pressureId);
        public Task<IEnumerable<PressureDTO>> GetUserPressure(string userId);
        public Task AddPressure(PressureToAddDTO pressure);
        public Task UpdatePressure(PressureDTO pressure);
        public Task DeletePressure(Guid pressureId);
    }
}
