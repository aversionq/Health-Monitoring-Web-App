using HealthMonitoringApp.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Interfaces
{
    public interface IBloodSugarBusiness
    {
        public Task<BloodSugarDTO> GetBloodSugarById(Guid bloodSuagrId);
        public Task<IEnumerable<BloodSugarDTO>> GetUserBloodSugar(string userId);
        public Task<BloodSugarDTO> GetLatestBloodSugar(string userId);
        public Task AddBloodSugar(BloodSugarDTO bloodSuagr);
        public Task UpdateBloodSugar(BloodSugarDTO bloodSuagr);
        public Task DeleteBloodSugar(Guid bloodSuagrId);
    }
}
