using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Core.Entities;
using HealthMonitoringApp.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringApp.Data.Implementations
{
    public class BloodSugarRepository : IBloodSugarRepository
    {
        private readonly HealthMonitoringDbContext _dbContext;

        public BloodSugarRepository(HealthMonitoringDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddBloodSugar(BloodSugar bloodSugar)
        {
            _dbContext.BloodSugars.Add(bloodSugar);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteBloodSugar(BloodSugar bloodSugar)
        {
            _dbContext.BloodSugars.Remove(bloodSugar);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BloodSugar> GetBloodSugarById(Guid bloodSugarId)
        {
            var bloodSugar = _dbContext.BloodSugars.Where(b => b.Id == bloodSugarId);
            return await bloodSugar.FirstOrDefaultAsync();
        }

        public async Task<BloodSugar> GetLatestBloodSugar(string userId)
        {
            var latestBloodSugar = _dbContext.BloodSugars.Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date);
            return await latestBloodSugar.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BloodSugar>> GetUserBloodSugar(string userId)
        {
            var userBloodSugar = _dbContext.BloodSugars.Where(b => b.UserId == userId);
            return await userBloodSugar.ToListAsync();
        }

        public async Task UpdateBloodSugar(BloodSugar updBloodSugar)
        {
            _dbContext.BloodSugars.Update(updBloodSugar);
            await _dbContext.SaveChangesAsync();
        }
    }
}
