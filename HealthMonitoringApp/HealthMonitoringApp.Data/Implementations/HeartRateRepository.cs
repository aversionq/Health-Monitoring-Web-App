using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Core.Entities;
using HealthMonitoringApp.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Data.Implementations
{
    public class HeartRateRepository : IHeartRateRepository
    {
        private readonly HealthMonitoringDbContext _dbContext;

        public HeartRateRepository(HealthMonitoringDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddHeartRate(HeartRate heartRate)
        {
            _dbContext.HeartRates.Add(heartRate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteHeartRate(HeartRate heartRate)
        {
            _dbContext.HeartRates.Remove(heartRate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<HeartRate> GetHeartRateById(Guid heartRateId)
        {
            var heartRate = _dbContext.HeartRates.Where(h => h.Id == heartRateId);
            return await heartRate.FirstOrDefaultAsync();
        }

        public async Task<HeartRate> GetLatestHeartRate(string userId)
        {
            var latestHeartRate = _dbContext.HeartRates.Where(h => h.UserId == userId)
                .OrderByDescending(h => h.Date);
            return await latestHeartRate.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HeartRate>> GetSortedPagedUserHeartRate(string userId, int page, string sortType)
        {
            int pageSize = 5;
            var heartRate = _dbContext.HeartRates.Where(p => p.UserId == userId);
            switch (sortType)
            {
                case "DateDesc":
                    heartRate = heartRate.OrderByDescending(x => x.Date);
                    break;
                case "DateAsc":
                    heartRate = heartRate.OrderBy(x => x.Date);
                    break;
                case "PulseDesc":
                    heartRate = heartRate.OrderByDescending(x => x.Pulse);
                    break;
                case "PulseAsc":
                    heartRate = heartRate.OrderBy(x => x.Pulse);
                    break;
                default:
                    heartRate = heartRate.OrderBy(x => x.Date);
                    break;
            }
            heartRate = heartRate.Skip((page - 1) * pageSize).Take(pageSize);

            return await heartRate.ToListAsync();
        }

        public async Task<IEnumerable<HeartRate>> GetUserHeartRate(string userId)
        {
            var userHeartRate = _dbContext.HeartRates
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Date);
            return await userHeartRate.ToListAsync();
        }

        public async Task<IEnumerable<HeartRate>> GetUserHeartRateByDateInterval(string userId, DateTime startDate, DateTime endDate)
        {
            var heartRate = _dbContext.HeartRates
                .Where(h => h.UserId == userId && h.Date >= startDate && h.Date <= endDate)
                .OrderBy(h => h.Date);
            return await heartRate.ToListAsync();
        }

        public async Task UpdateHeartRate(HeartRate updHeartRate)
        {
            _dbContext.HeartRates.Update(updHeartRate);
            await _dbContext.SaveChangesAsync();
        }
    }
}
