using HealthMonitoringApp.API.ResponseModels;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HealthMonitoringApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodSugarController : ControllerBase
    {
        private IBloodSugarBusiness _bloodSugarBusiness;
        private IConfiguration _configuration;
        private string? _userToken;

        public BloodSugarController(IBloodSugarBusiness bloodSugarBusiness, IConfiguration configuration)
        {
            _bloodSugarBusiness = bloodSugarBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getUserBloodSugar")]
        public async Task<ActionResult<List<BloodSugarDTO>>> GetUserBloodSugar()
        {
            try
            {
                var userId = await GetUserId();
                var userBloodSugar = await _bloodSugarBusiness.GetUserBloodSugar(userId);
                return Ok(userBloodSugar);
            }
            catch (Exception)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "User blood sugar not found",
                    ErrorCode = 900
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BloodSugarDTO>> GetBloodSugarById(Guid id)
        {
            try
            {
                var bloodSugar = await _bloodSugarBusiness.GetBloodSugarById(id);
                return bloodSugar;
            }
            catch (Exception)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = "Blood sugar with such id not found",
                    ErrorCode = 910
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddBloodSugar([FromBody] BloodSugarToAddDTO bloodSugar)
        {
            try
            {
                var userId = await GetUserId();
                var bloodSugarDTO = new BloodSugarDTO
                {
                    SugarValue = bloodSugar.SugarValue,
                    Date = bloodSugar.Date,
                    UserId = userId
                };
                await _bloodSugarBusiness.AddBloodSugar(bloodSugarDTO);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to add this resource",
                    ErrorCode = 920
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateBloodSugar([FromBody] BloodSugarDTO bloodSugar)
        {
            try
            {
                var userId = await GetUserId();
                bloodSugar.UserId = userId;
                await _bloodSugarBusiness.UpdateBloodSugar(bloodSugar);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 930
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveBloodSugar(Guid id)
        {
            try
            {
                await _bloodSugarBusiness.DeleteBloodSugar(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to update this resource",
                    ErrorCode = 940
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
