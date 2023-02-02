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

        public async Task<Pressure> GetPressureById(Guid pressureId)
        {
            var pressure = await _dbContext.Pressures.Where(x => x.Id == pressureId).FirstOrDefaultAsync();
            return pressure;
        }

        public async Task<IEnumerable<Pressure>> GetUserPressure(string userId)
        {
            var userPressure = await _dbContext.Pressures.Where(x => x.UserId == userId).ToListAsync();
            return userPressure;
        }

        public async Task UpdatePressure(Pressure updPressure)
        {
            _dbContext.Pressures.Update(updPressure);
            await _dbContext.SaveChangesAsync();
        }
    }
}
