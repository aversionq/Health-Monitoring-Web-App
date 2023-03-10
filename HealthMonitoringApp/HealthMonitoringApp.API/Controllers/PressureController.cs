using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using HealthMonitoringApp.API.ResponseModels;

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
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "User pressure not found",
                    ErrorCode = 700
                });
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
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "Pressure with such id not found",
                    ErrorCode = 710
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddPressure([FromBody] PressureToAddDTO pressure)
        {
            try
            {
                var userId = await GetUserId();
                var pressureDTO = new PressureDTO
                {
                    Systolic = pressure.Systolic,
                    Diastolic = pressure.Diastolic,
                    Pulse = pressure.Pulse,
                    Date = pressure.Date,
                    UserId = userId
                };
                await _pressureBusiness.AddPressure(pressureDTO);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to add this resource",
                    ErrorCode = 720
                });
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
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 730
                });
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
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 740
                });
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
                return Unauthorized(new ErrorResponse
                {
                    ErrorDescription = "JWT token is not valid",
                    ErrorCode = 750
                });
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
