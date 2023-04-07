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
using Microsoft.EntityFrameworkCore.Metadata;

namespace HealthMonitoringApp.Business.Implementations
{
    public class BloodSugarBusiness : IBloodSugarBusiness
    {
        private IBloodSugarRepository _repository;
        private Mapper _sugarMapper;

        public BloodSugarBusiness(IBloodSugarRepository repo)
        {
            _repository = repo;
            SetupMappers();
        }

        public async Task AddBloodSugar(BloodSugarDTO bloodSuagr)
        {
            var sugarEntity = _sugarMapper.Map<BloodSugarDTO, BloodSugar>(bloodSuagr);
            await _repository.AddBloodSugar(sugarEntity);
        }

        public async Task DeleteBloodSugar(Guid bloodSuagrId)
        {
            var suagrEntity = await _repository.GetBloodSugarById(bloodSuagrId);
            if (suagrEntity != null)
            {
                await _repository.DeleteBloodSugar(suagrEntity);
            }
            else
            {
                throw new Exception("Blood sugar with such id not found");
            }
        }

        public async Task<BloodSugarDTO> GetBloodSugarById(Guid bloodSuagrId)
        {
            try
            {
                var sugarEntity = await _repository.GetBloodSugarById(bloodSuagrId);
                var sugarDTO = _sugarMapper.Map<BloodSugar, BloodSugarDTO>(sugarEntity);
                return sugarDTO;
            }
            catch (Exception)
            {
                throw new Exception("Blood sugar with such id not found");
            }
        }

        public async Task<IEnumerable<BloodSugarDTO>> GetUserBloodSugar(string userId)
        {
            try
            {
                var userSugarEntity = await _repository.GetUserBloodSugar(userId);
                var userSugarDTO = _sugarMapper.Map<IEnumerable<BloodSugar>,
                    IEnumerable<BloodSugarDTO>>(userSugarEntity);
                return userSugarDTO;
            }
            catch (Exception)
            {
                throw new Exception("Blood sugar with such id not found");
            }
        }

        public async Task UpdateBloodSugar(BloodSugarDTO bloodSuagr)
        {
            var sugarEntity = _sugarMapper.Map<BloodSugarDTO, BloodSugar>(bloodSuagr);
            await _repository.UpdateBloodSugar(sugarEntity);
        }

        private void SetupMappers()
        {
            var sugarConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BloodSugar, BloodSugarDTO>().ReverseMap();
            });
            _sugarMapper = new Mapper(sugarConfig);
        }
    }
}
