using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using HealthMonitoringApp.API.ResponseModels;
using HealthMonitoringApp.Business.Implementations;
using HealthMonitoringApp.API.RequestModels;
using System.Text.Json;
using System.Text;

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
        private string? _userId;

        public PressureController(IPressureBusiness pressureBusiness, IConfiguration configuration)
        {
            _pressureBusiness = pressureBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getPatientPressure")]
        public async Task<ActionResult<List<PressureDTO>>> GetPatientPressure(string patientId)
        {
            try
            {
                var doctorId = await GetUserId();
                bool doctorCheck;
                using (var client = new HttpClient())
                {
                    var checkUri = _configuration["ServicesURI:AuthService"]
                        + "/api/Doctor/checkDoctorRequest";

                    var doctorReq = new DoctorCheckRequest
                    {
                        DoctorId = doctorId,
                        PatientId = patientId
                    };

                    var json = JsonSerializer.Serialize(doctorReq);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    await GetUserJWT();

                    client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _userToken);

                    var response = await client
                        .PostAsync(checkUri, data)
                        .Result.Content.ReadAsStringAsync();

                    doctorCheck = response == "true";
                }

                if (doctorCheck)
                {
                    var patientPressure = await _pressureBusiness.GetUserPressure(patientId);
                    return Ok(patientPressure);
                }
                else
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorDescription = "You are not allowed to get this user's medical data",
                        ErrorCode = 5000
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Something went wrong",
                    ErrorCode = 10000
                });
            }
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

        [HttpGet]
        [Route("getLatestPressure")]
        public async Task<ActionResult<PressureDTO>> GetLatestPressure()
        {
            try
            {
                var latestPressure = await _pressureBusiness
                    .GetLatestPressure(_userId ?? await GetUserId());
                return Ok(latestPressure);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = ex.Message,
                    ErrorCode = 750
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
                _userId = await response.Content.ReadAsStringAsync();
                return _userId;
            }
        }
    }
}
