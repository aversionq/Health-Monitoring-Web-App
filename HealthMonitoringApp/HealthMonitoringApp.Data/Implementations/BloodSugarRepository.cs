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

        public async Task<IEnumerable<BloodSugar>> GetSortedPagedUserBloodSugar(string userId, int page, string sortType)
        {
            int pageSize = 5;
            var sugar = _dbContext.BloodSugars.Where(p => p.UserId == userId);
            switch (sortType)
            {
                case "DateDesc":
                    sugar = sugar.OrderByDescending(x => x.Date);
                    break;
                case "DateAsc":
                    sugar = sugar.OrderBy(x => x.Date);
                    break;
                case "SugarDesc":
                    sugar = sugar.OrderByDescending(x => x.SugarValue); 
                    break;
                case "SugarAsc":
                    sugar = sugar.OrderBy(x => x.SugarValue);
                    break;
                default:
                    sugar = sugar.OrderBy(x => x.Date);
                    break;
            }
            sugar = sugar.Skip((page - 1) * pageSize).Take(pageSize);

            return await sugar.ToListAsync();
        }

        public async Task<IEnumerable<BloodSugar>> GetUserBloodSugar(string userId)
        {
            var userBloodSugar = _dbContext.BloodSugars
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Date);
            return await userBloodSugar.ToListAsync();
        }

        public async Task<IEnumerable<BloodSugar>> GetUserBloodSugarByDateInterval(string userId, DateTime startDate, DateTime endDate)
        {
            var sugar = _dbContext.BloodSugars
                .Where(b => b.UserId == userId && b.Date >= startDate && b.Date <= endDate)
                .OrderBy(b => b.Date);
            return await sugar.ToListAsync();
        }

        public async Task UpdateBloodSugar(BloodSugar updBloodSugar)
        {
            _dbContext.BloodSugars.Update(updBloodSugar);
            await _dbContext.SaveChangesAsync();
        }
    }
}
