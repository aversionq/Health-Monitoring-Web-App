using HealthMonitoringApp.Business.DTOs;

namespace HealthMonitoringApp.Business.Interfaces
{
    public interface IPressureBusiness
    {
        public Task<PressureDTO> GetPressureById(Guid pressureId);
        public Task<IEnumerable<PressureDTO>> GetUserPressure(string userId);
        public Task<IEnumerable<PressureDTO>> GetSortedPagedUserPressure(string userId, int page, string sortType);
        public Task<IEnumerable<PressureDTO>> GetUserPressureByDateInterval(string userId, DateTime startDate, DateTime endDate);
        public Task<PressureDTO> GetLatestPressure(string userId);
        public Task AddPressure(PressureDTO pressure);
        public Task UpdatePressure(PressureDTO pressure);
        public Task DeletePressure(Guid pressureId);
    }
}
