using HealthMonitoringApp.API.ResponseModels;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Implementations;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HealthMonitoringApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeartRateController : ControllerBase
    {
        private IHeartRateBusiness _heartRateBusiness;
        private IConfiguration _configuration;
        private string? _userToken;
        private string? _userId;

        public HeartRateController(IHeartRateBusiness heartRateBusiness, IConfiguration configuration)
        {
            _heartRateBusiness = heartRateBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getUserHeartRate")]
        public async Task<ActionResult<List<HeartRateDTO>>> GetUserHeartRate()
        {
            try
            {
                var userId = await GetUserId();
                var userHeartRate = await _heartRateBusiness.GetUserHeartRate(userId);
                return Ok(userHeartRate);
            }
            catch (Exception)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "User heart rate not found",
                    ErrorCode = 800
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HeartRateDTO>> GetHeartRateById(Guid id)
        {
            try
            {
                var heartRate = await _heartRateBusiness.GetHeartRateById(id);
                return heartRate;
            }
            catch (Exception)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "Heart rate with such id not found",
                    ErrorCode = 810
                });
            }
        }

        [HttpGet]
        [Route("getLatestHeartRate")]
        public async Task<ActionResult<HeartRateDTO>> GetLatestHeartRate()
        {
            try
            {
                var latestHeartRate = await _heartRateBusiness
                    .GetLatestHeartRate(_userId ?? await GetUserId());
                return Ok(latestHeartRate);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = ex.Message,
                    ErrorCode = 850
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddHeartRate([FromBody] HeartRateToAddDTO heartRate)
        {
            try
            {
                var userId = await GetUserId();
                var heartRateDTO = new HeartRateDTO
                {
                    Pulse = heartRate.Pulse,
                    Date = heartRate.Date,
                    UserId = userId
                };
                await _heartRateBusiness.AddHeartRate(heartRateDTO);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to add this resource",
                    ErrorCode = 820
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateHeartRate([FromBody] HeartRateDTO heartRate)
        {
            try
            {
                var userId = await GetUserId();
                heartRate.UserId = userId;
                await _heartRateBusiness.UpdateHeartRate(heartRate);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 830
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveHeartRate(Guid id)
        {
            try
            {
                await _heartRateBusiness.DeleteHeartRate(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 840
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
                _userId = await response.Content.ReadAsStringAsync();
                return _userId;
            }
        }
    }
}
