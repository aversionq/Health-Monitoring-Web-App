using HealthMonitoringApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Application.Interfaces
{
    public interface IBloodSugarRepository
    {
        public Task<BloodSugar> GetBloodSugarById(Guid bloodSugarId);
        public Task<IEnumerable<BloodSugar>> GetUserBloodSugar(string userId);
        public Task<BloodSugar> GetLatestBloodSugar(string userId);
        public Task AddBloodSugar(BloodSugar bloodSugar);
        public Task UpdateBloodSugar(BloodSugar updBloodSugar);
        public Task DeleteBloodSugar(BloodSugar bloodSugar);
    }
}
