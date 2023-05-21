using HealthMonitoringApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Application.Interfaces
{
    public interface IHeartRateRepository
    {
        public Task<HeartRate> GetHeartRateById(Guid heartRateId);
        public Task<IEnumerable<HeartRate>> GetUserHeartRate(string userId);
        public Task<IEnumerable<HeartRate>> GetSortedPagedUserHeartRate(string userId, int page, string sortType);
        public Task<IEnumerable<HeartRate>> GetUserHeartRateByDateInterval(string userId, DateTime startDate, DateTime endDate);
        public Task<HeartRate> GetLatestHeartRate(string userId);
        public Task AddHeartRate(HeartRate heartRate);
        public Task UpdateHeartRate(HeartRate updHeartRate);
        public Task DeleteHeartRate(HeartRate heartRate);
    }
}
