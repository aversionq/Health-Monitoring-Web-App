using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoringApp.Core.Entities;

namespace HealthMonitoringApp.Business.Implementations
{
    public class PressureBusiness : IPressureBusiness
    {
        private IPressureRepository _repository;
        private Mapper _pressureMapper;

        public PressureBusiness(IPressureRepository repo)
        {
            _repository = repo;
            SetupMappers();
        }

        public async Task AddPressure(PressureDTO pressure)
        {
            var pressureEntity = _pressureMapper.Map<PressureDTO, Pressure>(pressure);
            await _repository.AddPressure(pressureEntity);
        }

        public async Task DeletePressure(Guid pressureId)
        {
            var pressureEntity = await _repository.GetPressureById(pressureId);
            if (pressureEntity != null)
            {
                await _repository.DeletePressure(pressureEntity);
            }
            else
            {
                throw new Exception("Pressure with such id not found");
            }
        }

        public async Task<PressureDTO> GetLatestPressure(string userId)
        {
            try
            {
                var latestPressureEntity = await _repository.GetLatestPressure(userId);
                var latestPressureDTO = _pressureMapper
                    .Map<Pressure, PressureDTO>(latestPressureEntity);
                return latestPressureDTO;
            }
            catch (Exception)
            {
                throw new Exception("Latest pressure for this user not found");
            }
        }

        public async Task<PressureDTO> GetPressureById(Guid pressureId)
        {
            try
            {
                var pressureEntity = await _repository.GetPressureById(pressureId);
                var pressureDTO = _pressureMapper.Map<Pressure, PressureDTO>(pressureEntity);
                return pressureDTO;
            }
            catch (Exception)
            {
                throw new Exception("Pressure with such id not found");
            }
        }

        public async Task<IEnumerable<PressureDTO>> GetUserPressure(string userId)
        {
            try
            {
                var userPressureEntity = await _repository.GetUserPressure(userId);
                var userPressureDTO = _pressureMapper.Map<IEnumerable<Pressure>, 
                    IEnumerable<PressureDTO>>(userPressureEntity);
                return userPressureDTO;
            }
            catch (Exception)
            {
                throw new Exception("Pressure with such id not found");
            }
        }

        public async Task UpdatePressure(PressureDTO pressure)
        {
            var pressureEntity = _pressureMapper.Map<PressureDTO, Pressure>(pressure);
            await _repository.UpdatePressure(pressureEntity);
        }

        private void SetupMappers()
        {
            var pressureMapperConfig = new MapperConfiguration(
                cfg => cfg.CreateMap<PressureDTO, Pressure>().ReverseMap()
                );
            _pressureMapper = new Mapper(pressureMapperConfig);
        }
    }
}
