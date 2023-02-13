using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace HealthMonitoringApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PressureController : ControllerBase
    {
        private IPressureBusiness _pressureBusiness;
        private IConfiguration _configuration;
        private string? _userToken;

        public PressureController(IPressureBusiness pressureBusiness, IConfiguration configuration)
        {
            _pressureBusiness = pressureBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getUserPressure")]
        public async Task<ActionResult<List<PressureDTO>>> GetUserPressure()
        {
            try
            {
                var userId = await GetUserId();
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
                var userId = await GetUserId();
                pressure.UserId = userId;
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
                var userId = await GetUserId();
                pressure.UserId = userId;
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

        [HttpPost]
        [Route("getUserJWT")]
        public async Task<ActionResult> GetUserJWT()
        {
            try
            {
                var req = Request;
                var headers = req.Headers;
                var authHeader = headers.Authorization;
                var token = authHeader.ToString().Split(' ')[1];

                _userToken = token;
                return Ok();
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        private async Task<string> GetUserId()
        {
            if (_userToken == null)
            {
                await GetUserJWT();
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration["ServicesURI:AuthService"]);
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _userToken);
                var response = await client.GetAsync("api/User/getCurrentUserId");
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
