using HealthMonitoringApp.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Business.Interfaces
{
    public interface IHeartRateBusiness
    {
        public Task<HeartRateDTO> GetHeartRateById(Guid heartRateId);
        public Task<IEnumerable<HeartRateDTO>> GetUserHeartRate(string userId);
        public Task<IEnumerable<HeartRateDTO>> GetSortedPagedUserHeartRate(string userId, int page, string sortType);
        public Task<IEnumerable<HeartRateDTO>> GetUserHeartRateByDateInterval(string userId, DateTime startDate, DateTime endDate);
        public Task<HeartRateDTO> GetLatestHeartRate(string userId);
        public Task AddHeartRate(HeartRateDTO heartRate);
        public Task UpdateHeartRate(HeartRateDTO heartRate);
        public Task DeleteHeartRate(Guid heartRateId);
    }
}
