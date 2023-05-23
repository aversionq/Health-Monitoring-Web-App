using AuthServer.DatabaseContext;
using AuthServer.DTOs;
using AuthServer.Models;
using AuthServer.RequestModels;
using AuthServer.ResponseModels;
using AuthServer.Roles;
using AuthServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using System.Security.Policy;

namespace AuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private UsersDbContext _dbContext;
        private PictureUploadService _pictureUploadService;

        public DoctorController(UserManager<ApplicationUser> userManager,
            UsersDbContext ctx, PictureUploadService picService)
        {
            _userManager = userManager;
            _dbContext = ctx;
            _pictureUploadService = picService;
        }

        [HttpGet]
        [Route("getAllDoctors")]
        public async Task<ActionResult<List<ApplicationUserDTO>>> GetAllDoctors()
        {
            var roleId = await _dbContext.AspNetRoles
                .Where(x => x.Name == UserRoles.Doctor)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var doctors = await _dbContext.AspNetRoles
                .Where(x => x.Id == roleId)
                .SelectMany(u => u.Users)
                .Include(r => r.Roles)
                .ToListAsync();

            List<ApplicationUserDTO> doctorsDTO = new List<ApplicationUserDTO>();
            foreach (var doctorEntity in doctors)
            {
                doctorsDTO.Add(await MapUserDoctorDTO(doctorEntity));
            }

            return doctorsDTO;
        }

        [Authorize(Roles = UserRoles.Doctor)]
        [HttpGet]
        [Route("getDoctorPatients")]
        public async Task<ActionResult<List<PatientDTO>>> GetDoctorPatients()
        {
            var userId = await GetCurrentUserId();
            var patients = await _dbContext.DoctorPatients
                .Where(x => x.DoctorId == userId)
                .Select(x => x.User)
                .ToListAsync();

            List<PatientDTO> patientsDTO = new List<PatientDTO>();
            foreach (var patient in patients)
            {
                patientsDTO.Add(new PatientDTO
                {
                    PatientId = patient.Id,
                    PatientFirstName = patient.FirstName,
                    PatientLastName = patient.LastName,
                    Username = patient.UserName
                });
            }
            //foreach (var patientId in patientIds)
            //{
            //    var newPatient = await _dbContext.AspNetUsers.Where(x => x.Id == patientId).FirstOrDefaultAsync();
            //}
            return Ok(patientsDTO);
        }

        [Authorize(Roles = UserRoles.Doctor)]
        [HttpGet]
        [Route("getPatientBioData")]
        public async Task<ActionResult<PatientBioData>> GetPatientBioData(string patientId)
        {
            var doctorId = await GetCurrentUserId();
            var doctorCheck = await _dbContext.DoctorPatients
                    .Where(x => x.DoctorId == doctorId && x.UserId == patientId)
                    .FirstOrDefaultAsync();
            if (doctorCheck is not null)
            {
                var patient = await _dbContext.AspNetUsers
                    .Where(x => x.Id == patientId)
                    .FirstOrDefaultAsync();
                var bioData = new PatientBioData
                {
                    UserHeight = patient.Height,
                    UserWeight = patient.Weight,
                    Age = patient.DateOfBirth is null ? null :
                        (int)((DateTime.Now - patient.DateOfBirth.Value).TotalDays / 365.25),
                    Gender = patient.Gender is null ? null : ((GenderType.GenderTypes)patient.Gender).ToString()
                };

                return Ok(bioData);
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "You dont have permission to get this user bio data (it is not your patient)",
                    ErrorCode = 77000
                });
            }
        }

        [HttpPost]
        [Route("requestDoctorRole")]
        public async Task<ActionResult> RequestDoctorRole([FromForm] DoctorRoleRequest doctorRoleRequest)
        {
            try
            {
                if (doctorRoleRequest.PassportImage.files.Length > 0)
                {
                    var userId = await GetCurrentUserId();
                    var passportImage = await _pictureUploadService.UploadImage(doctorRoleRequest.PassportImage);
                    var diplomaImage = await _pictureUploadService.UploadImage(doctorRoleRequest.DiplomaImage);
                    if (passportImage is not null && diplomaImage is not null)
                    {
                        _dbContext.DoctorRequests.Add(new DoctorRequest
                        {
                            UserId = doctorRoleRequest.UserId,
                            PassportPicture = passportImage,
                            DiplomaPicture = diplomaImage
                        });
                        await _dbContext.SaveChangesAsync();

                        return Ok(new
                        {
                            Message = "Doctor role succesfully requested! Wait for moderators to check it"
                        });
                    }
                    else
                    {
                        return BadRequest(new ErrorResponse
                        {
                            ErrorDescription = "You need to upload both photos of " +
                            "your passport and diploma to request a doctor role",
                            ErrorCode = 2000
                        });
                    }
                }
                else
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorDescription = "There is no picture to upload",
                        ErrorCode = 1300
                    });
                }
            }
            catch (JsonReaderException)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Failed to upload profile picture",
                    ErrorCode = 1400
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorDescription = ex.Message,
                    ErrorCode = 1500
                });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("acceptDoctorRequest")]
        public async Task<ActionResult> AcceptDoctorRequest(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Doctor);
                _dbContext.DoctorRequests.Remove(new DoctorRequest
                {
                    UserId = userId
                });
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"User with id: {userId} now has a doctor role"
                });
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "User not found",
                    ErrorCode = 680
                });
            }
        }

        [Authorize(Roles = UserRoles.Doctor)]
        [HttpPost]
        [Route("checkDoctorRequest")]
        public async Task<ActionResult<bool>> CheckDoctorRequest([FromBody] DoctorCheckRequest dcr)
        {
            try
            {
                var doctorCheck = await _dbContext.DoctorPatients
                    .Where(x => x.DoctorId == dcr.DoctorId && x.UserId == dcr.PatientId)
                    .FirstOrDefaultAsync();
                return Ok(doctorCheck is not null);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = ex.Message,
                    ErrorCode = 3500
                });
            }
        }

        private async Task<string> GetCurrentUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private async Task<ApplicationUserDTO> MapUserDoctorDTO(AspNetUser user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var roles = await _userManager.GetRolesAsync(appUser);
            string userRole;
            if (roles.Contains(UserRoles.Admin))
            {
                userRole = UserRoles.Admin;
            }
            else if (roles.Contains(UserRoles.Doctor))
            {
                userRole = UserRoles.Doctor;
            }
            else
            {
                userRole = UserRoles.DefaultUser;
            }
            ApplicationUserDTO userDTO = new ApplicationUserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRole,
                DateOfBirth = user.DateOfBirth,
                RegistrationDate = user.RegistrationDate,
                Email = user.Email,
                Username = user.UserName,
                Age = user.DateOfBirth is null ? null : (int)((DateTime.Now - user.DateOfBirth.Value).TotalDays / 365.25),
                Weight = user.Weight,
                Height = user.Height,
                Gender = user.Gender is null ? null : ((GenderType.GenderTypes)user.Gender).ToString()
            };

            return userDTO;
        }
    }
}
