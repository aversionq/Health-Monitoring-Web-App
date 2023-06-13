using HealthMonitoringApp.API.RequestModels;
using HealthMonitoringApp.API.ResponseModels;
using HealthMonitoringApp.Business.DTOs;
using HealthMonitoringApp.Business.Enums;
using HealthMonitoringApp.Business.Implementations;
using HealthMonitoringApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HealthMonitoringApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BloodSugarController : ControllerBase
    {
        private IBloodSugarBusiness _bloodSugarBusiness;
        private IConfiguration _configuration;
        private string? _userToken;
        private string? _userId;

        public BloodSugarController(IBloodSugarBusiness bloodSugarBusiness, IConfiguration configuration)
        {
            _bloodSugarBusiness = bloodSugarBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getPatientBloodSugar")]
        public async Task<ActionResult<List<BloodSugarDTO>>> GetPatientBloodSugar(string patientId)
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
                    var patientBloodSugar = await _bloodSugarBusiness.GetUserBloodSugar(patientId);
                    return Ok(patientBloodSugar);
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
        [Route("getUserBloodSugar")]
        public async Task<ActionResult<List<BloodSugarDTO>>> GetUserBloodSugar()
        {
            try
            {
                var userId = await GetUserId();
                var userBloodSugar = await _bloodSugarBusiness.GetUserBloodSugar(userId);
                // TODO: Refactor receiving userId!!!

                //var test = this.User;
                //var iddd = test.FindFirstValue(ClaimTypes.NameIdentifier);
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

        [HttpGet]
        [Route("getSortedPagedUserBloodSugar")]
        public async Task<ActionResult<List<BloodSugarDTO>>> GetSortedPagedUserBloodSugar(int page, string sortType)
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var enumValue = Enum.Parse<SortTypes.BloodSugarSort>(sortType);
                var sugar = await _bloodSugarBusiness.GetSortedPagedUserBloodSugar(userId, page, sortType);
                return Ok(sugar);
            }
            catch (ArgumentException)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Wrong enum type for sort",
                    ErrorCode = 50000
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to get sorted and paged blood sugar",
                    ErrorCode = 17000
                });
            }
        }

        [HttpGet]
        [Route("getUserBloodSugarByDateInterval")]
        public async Task<ActionResult<List<BloodSugarDTO>>> GetUserBloodSugarByDateInterval(DateTime startDate, DateTime endDate)
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var sugar = await _bloodSugarBusiness.GetUserBloodSugarByDateInterval(userId, startDate, endDate);
                return Ok(sugar);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to get blood sugar in this interval",
                    ErrorCode = 27000
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

        [HttpGet]
        [Route("getLatestBloodSugar")]
        public async Task<ActionResult<BloodSugarDTO>> GetLatestBloodSugar()
        {
            try
            {
                var latestBloodSugar = await _bloodSugarBusiness
                    .GetLatestBloodSugar(_userId ?? await GetUserId());
                return Ok(latestBloodSugar);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponse
                {
                    ErrorDescription = ex.Message,
                    ErrorCode = 950
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
                _userId = await response.Content.ReadAsStringAsync();
                return _userId;
            }
        }
    }
}
