using HealthMonitoringApp.Business.DTOs;

namespace HealthMonitoringApp.Business.Interfaces
{
    public interface IPressureBusiness
    {
        public Task<PressureDTO> GetPressureById(Guid pressureId);
        public Task<IEnumerable<PressureDTO>> GetUserPressure(string userId);
        public Task<PressureDTO> GetLatestPressure(string userId);
        public Task AddPressure(PressureDTO pressure);
        public Task UpdatePressure(PressureDTO pressure);
        public Task DeletePressure(Guid pressureId);
    }
}
