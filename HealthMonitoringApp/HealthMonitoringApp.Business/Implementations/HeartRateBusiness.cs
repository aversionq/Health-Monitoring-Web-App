﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using HealthMonitoringApp.Core.Entities;

namespace HealthMonitoringApp.Business.Implementations
{
    public class HeartRateBusiness : IHeartRateBusiness
    {
        private IHeartRateRepository _repository;
        private Mapper _pulseMapper;

        public HeartRateBusiness(IHeartRateRepository repo)
        {
            _repository = repo;
            SetupMappers();
        }

        public async Task AddHeartRate(HeartRateDTO heartRate)
        {
            var pulseEntity = _pulseMapper.Map<HeartRateDTO, HeartRate>(heartRate);
            await _repository.AddHeartRate(pulseEntity);
        }

        public async Task DeleteHeartRate(Guid heartRateId)
        {
            var pulseEntity = await _repository.GetHeartRateById(heartRateId);
            if (pulseEntity != null)
            {
                await _repository.DeleteHeartRate(pulseEntity);
            }
            else
            {
                throw new Exception("Heart rate with such id not found");
            }
        }

        public async Task<HeartRateDTO> GetHeartRateById(Guid heartRateId)
        {
            try
            {
                var pulseEntity = await _repository.GetHeartRateById(heartRateId);
                var pulseDTO = _pulseMapper.Map<HeartRate, HeartRateDTO>(pulseEntity);
                return pulseDTO;
            }
            catch (Exception)
            {
                throw new Exception("Heart rate with such id not found");
            }
        }

        public async Task<IEnumerable<HeartRateDTO>> GetUserHeartRate(string userId)
        {
            try
            {
                var userPulseEntity = await _repository.GetUserHeartRate(userId);
                var userPulseDTO = _pulseMapper.Map<IEnumerable<HeartRate>,
                    IEnumerable<HeartRateDTO>>(userPulseEntity);
                return userPulseDTO;
            }
            catch (Exception)
            {
                throw new Exception("Heart rate with such id not found");
            }
        }

        public async Task UpdateHeartRate(HeartRateDTO heartRate)
        {
            var pulseEntity = _pulseMapper.Map<HeartRateDTO, HeartRate>(heartRate);
            await _repository.UpdateHeartRate(pulseEntity);
        }

        private void SetupMappers()
        {
            var pulseConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HeartRate, HeartRateDTO>().ReverseMap();
            });
            _pulseMapper = new Mapper(pulseConfig);
        }
    }
}