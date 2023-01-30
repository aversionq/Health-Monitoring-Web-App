using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoringApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PressureController : ControllerBase
    {
        private IPressureBusiness _pressureBusiness;

        public PressureController(IPressureBusiness pressureBusiness)
        {
            _pressureBusiness = pressureBusiness;
        }

        [HttpGet]
        [Route("getUserPressure")]
        public async Task<ActionResult<List<PressureDTO>>> GetUserPressure(string userId)
        {
            try
            {
                var userPressure = await _pressureBusiness.GetUserPressure(userId);
                return Ok(userPressure);
            }
            catch (Exception)
            {
                return NotFound("User pressure not found");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PressureDTO>> GetPressureById(Guid id)
        {
            try
            {
                var pressure = await _pressureBusiness.GetPressureById(id);
                return pressure;
            }
            catch (Exception)
            {
                return NotFound("Pressure with such id not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddPressure([FromBody] PressureToAddDTO pressure)
        {
            try
            {
                await _pressureBusiness.AddPressure(pressure);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Not able to add this resource");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdatePressure([FromBody] PressureDTO pressure)
        {
            try
            {
                await _pressureBusiness.UpdatePressure(pressure);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Not able to update this resource");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemovePressure(Guid id)
        {
            try
            {
                await _pressureBusiness.DeletePressure(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Not able to update this resource");
            }
        }
    }
}
