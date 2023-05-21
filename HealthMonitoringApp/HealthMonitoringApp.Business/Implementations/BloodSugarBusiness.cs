using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using HealthMonitoringApp.Business.Services;
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
                DetermineBloodSugarState(sugarDTO);
                return sugarDTO;
            }
            catch (Exception)
            {
                throw new Exception("Blood sugar with such id not found");
            }
        }

        public async Task<BloodSugarDTO> GetLatestBloodSugar(string userId)
        {
            try
            {
                var latestBloodSugarEntity = await _repository.GetLatestBloodSugar(userId);
                var latestBloodSugarDTO = _sugarMapper
                    .Map<BloodSugar, BloodSugarDTO>(latestBloodSugarEntity);
                DetermineBloodSugarState(latestBloodSugarDTO);
                return latestBloodSugarDTO;
            }
            catch (Exception)
            {
                throw new Exception("Latest blood sugar for this user not found");
            }
        }

        public async Task<IEnumerable<BloodSugarDTO>> GetSortedPagedUserBloodSugar(string userId, int page, string sortType)
        {
            try
            {
                var sugar = await _repository.GetSortedPagedUserBloodSugar(userId, page, sortType);
                var sugarDTO = _sugarMapper
                    .Map<IEnumerable<BloodSugar>, IEnumerable<BloodSugarDTO>>(sugar);
                sugarDTO.ToList().ForEach(x => DetermineBloodSugarState(x));
                return sugarDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BloodSugarDTO>> GetUserBloodSugar(string userId)
        {
            try
            {
                var userSugarEntity = await _repository.GetUserBloodSugar(userId);
                var userSugarDTO = _sugarMapper.Map<IEnumerable<BloodSugar>,
                    IEnumerable<BloodSugarDTO>>(userSugarEntity);
                userSugarDTO.ToList().ForEach(x => DetermineBloodSugarState(x));
                return userSugarDTO;
            }
            catch (Exception)
            {
                throw new Exception("Blood sugar with such id not found");
            }
        }

        public async Task<IEnumerable<BloodSugarDTO>> GetUserBloodSugarByDateInterval(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var sugar = await _repository.GetUserBloodSugarByDateInterval(userId, startDate, endDate);
                var sugarDTO = _sugarMapper
                    .Map<IEnumerable<BloodSugar>, IEnumerable<BloodSugarDTO>>(sugar);
                sugarDTO.ToList().ForEach(x => DetermineBloodSugarState(x));
                return sugarDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateBloodSugar(BloodSugarDTO bloodSuagr)
        {
            var sugarEntity = _sugarMapper.Map<BloodSugarDTO, BloodSugar>(bloodSuagr);
            await _repository.UpdateBloodSugar(sugarEntity);
        }

        private void DetermineBloodSugarState(BloodSugarDTO sugar)
        {
            sugar.MedicalState = MedicalStateHandler.GetUserBloodSugarState(sugar.SugarValue).ToString();
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
