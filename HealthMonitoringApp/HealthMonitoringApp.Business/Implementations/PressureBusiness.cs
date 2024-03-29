﻿using HealthMonitoringApp.Application.Interfaces;
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
using HealthMonitoringApp.Business.Services;

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
                DeterminePressureState(latestPressureDTO);
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
                DeterminePressureState(pressureDTO);
                return pressureDTO;
            }
            catch (Exception)
            {
                throw new Exception("Pressure with such id not found");
            }
        }

        public async Task<IEnumerable<PressureDTO>> GetSortedPagedUserPressure(string userId, int page, string sortType)
        {
            try
            {
                var pressure = await _repository.GetSortedPagedUserPressure(userId, page, sortType);
                var pressureDTO = _pressureMapper
                    .Map<IEnumerable<Pressure>, IEnumerable<PressureDTO>>(pressure);
                pressureDTO.ToList().ForEach(x => DeterminePressureState(x));
                return pressureDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PressureDTO>> GetUserPressure(string userId)
        {
            try
            {
                var userPressureEntity = await _repository.GetUserPressure(userId);
                var userPressureDTO = _pressureMapper.Map<IEnumerable<Pressure>, 
                    IEnumerable<PressureDTO>>(userPressureEntity);
                userPressureDTO.ToList().ForEach(x => DeterminePressureState(x));
                return userPressureDTO;
            }
            catch (Exception)
            {
                throw new Exception("Pressure with such id not found");
            }
        }

        public async Task<IEnumerable<PressureDTO>> GetUserPressureByDateInterval(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var pressure = await _repository.GetUserPressureByDateInterval(userId, startDate, endDate);
                var pressureDTO = _pressureMapper
                    .Map<IEnumerable<Pressure>, IEnumerable<PressureDTO>>(pressure);
                pressureDTO.ToList().ForEach(x => DeterminePressureState(x));
                return pressureDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdatePressure(PressureDTO pressure)
        {
            var pressureEntity = _pressureMapper.Map<PressureDTO, Pressure>(pressure);
            await _repository.UpdatePressure(pressureEntity);
        }

        private void DeterminePressureState(PressureDTO pressure)
        {
            pressure.MedicalState = MedicalStateHandler
                .GetUserPressureState(pressure.Systolic)
                .ToString();
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
