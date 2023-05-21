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
    public class PressureRepository : IPressureRepository
    {
        private readonly HealthMonitoringDbContext _dbContext;

        public PressureRepository(HealthMonitoringDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddPressure(Pressure pressure)
        {
            _dbContext.Pressures.Add(pressure);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePressure(Pressure pressure)
        {
            _dbContext.Pressures.Remove(pressure);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Pressure> GetLatestPressure(string userId)
        {
            var latestPressure = _dbContext.Pressures.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date);
            return await latestPressure.FirstOrDefaultAsync();
        }

        public async Task<Pressure> GetPressureById(Guid pressureId)
        {
            var pressure = await _dbContext.Pressures.Where(x => x.Id == pressureId).FirstOrDefaultAsync();
            return pressure;
        }

        public async Task<IEnumerable<Pressure>> GetSortedPagedUserPressure(string userId, int page, string sortType)
        {
            int pageSize = 5;
            var pressure = _dbContext.Pressures.Where(p => p.UserId == userId);
            switch (sortType)
            {
                case "DateDesc":
                    pressure = pressure.OrderByDescending(x => x.Date);
                    break;
                case "DateAsc":
                    pressure = pressure.OrderBy(x => x.Date);
                    break;
                case "SystolicDesc":
                    pressure = pressure.OrderByDescending(x => x.Systolic);
                    break;
                case "SystolicAsc":
                    pressure = pressure.OrderBy(x => x.Systolic);
                    break;
                case "DiastolicDesc":
                    pressure = pressure.OrderByDescending(x => x.Diastolic);
                    break;
                case "DiastolicAsc":
                    pressure = pressure.OrderBy(x => x.Diastolic);
                    break;
                default:
                    pressure = pressure.OrderBy(x => x.Date);
                    break;
            }
            pressure = pressure.Skip((page - 1) * pageSize).Take(pageSize);

            return await pressure.ToListAsync();
        }

        public async Task<IEnumerable<Pressure>> GetUserPressure(string userId)
        {
            var userPressure = _dbContext.Pressures
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Date);
            return await userPressure.ToListAsync();
        }

        public async Task<IEnumerable<Pressure>> GetUserPressureByDateInterval(string userId, DateTime startDate, DateTime endDate)
        {
            var pressure = _dbContext.Pressures
                .Where(p => p.UserId == userId && p.Date >= startDate && p.Date <= endDate)
                .OrderBy(p => p.Date);
            return await pressure.ToListAsync();
        }

        public async Task UpdatePressure(Pressure updPressure)
        {
            _dbContext.Pressures.Update(updPressure);
            await _dbContext.SaveChangesAsync();
        }
    }
}
