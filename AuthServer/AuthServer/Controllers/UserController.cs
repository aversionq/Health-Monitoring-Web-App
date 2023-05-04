using AuthServer.DatabaseContext;
using AuthServer.DTOs;
using AuthServer.Models;
using AuthServer.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using AuthServer.ResponseModels;
using Microsoft.EntityFrameworkCore;
using AuthServer.Roles;
using AutoMapper;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using AuthServer.Services;

namespace AuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private UsersDbContext _dbContext;
        private Mapper _userMapper;
        private PictureUploadService _pictureUploadService;

        public UserController(UserManager<ApplicationUser> userManager, 
            UsersDbContext context, PictureUploadService pictureUpload)
        {
            _userManager= userManager;
            _dbContext = context;
            _pictureUploadService = pictureUpload;
            SetupMapper();
        }

        [HttpPost]
        [Route("becomeDoctorsPatient")]
        public async Task<ActionResult> LetDoctorAccessMedicalData(string doctorId)
        {
            var doctors = await _userManager.GetUsersInRoleAsync(UserRoles.Doctor);
            var doctorIds = doctors.Select(x => x.Id).ToList();
            if (doctorIds.Contains(doctorId))
            {
                _dbContext.DoctorPatients.Add(new DoctorPatient
                {
                    DoctorId = doctorId,
                    UserId = await GetCurrentUserId()
                });
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "You successfully gave access to your medical data to the doctor"
                });
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "User you are trying to give data access is not a doctor",
                    ErrorCode = 3000
                });
            }
        }

        [HttpGet]
        [Route("getCurrentUserId")]
        public async Task<string> GetCurrentUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        [HttpGet]
        [Route("getCurrentUser")]
        public async Task<ApplicationUserDTO> GetCurrentUser()
        {
            var userId = await GetCurrentUserId();
            return await GetUserById(userId);
        }

        [HttpGet("{userId}")]
        public async Task<ApplicationUserDTO> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
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
            //var userDTO = _userMapper.Map<ApplicationUser, ApplicationUserDTO>(user);
            return userDTO;
        }

        [HttpGet]
        [Route("getUserBMI")]
        public async Task<ActionResult<BmiResponse>> GetUserBmi(string userId)
        {
            var bmiInfo = await _dbContext.AspNetUsers
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.Height,
                    x.Weight
                }).FirstOrDefaultAsync();
            var userBmi = new BmiResponse
            {
                UserHeight = bmiInfo.Height,
                UserWeight = bmiInfo.Weight
            };
            Console.Out.WriteLine(userBmi.Bmi);

            return Ok(userBmi);
        }
 
        [HttpPatch]
        [Route("changeWeight")]
        public async Task<ActionResult> EditUserWeight([FromBody] MetricsChange metrics)
        {
            try
            {
                var userId = await GetCurrentUserId();
                var user = new AspNetUser
                {
                    Id = userId,
                    Weight = metrics.Value
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user).Property(x => x.Weight).IsModified = true;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to change user's weight",
                    ErrorCode = 660
                });
            }
        }

        [HttpPatch]
        [Route("changeHeight")]
        public async Task<ActionResult> EditUserHeight([FromBody] MetricsChange metrics)
        {
            try
            {
                var userId = await GetCurrentUserId();
                var user = new AspNetUser
                {
                    Id = userId,
                    Height = metrics.Value
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user).Property(x => x.Height).IsModified = true;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to change user's height",
                    ErrorCode = 660
                });
            }
        }

        [HttpPatch]
        [Route("changeUsername")]
        public async Task<ActionResult> EditUsername(string newUsername)
        {
            if (await _userManager.FindByNameAsync(newUsername) != null)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "This username was already taken",
                    ErrorCode = 670
                });
            }

            var user = new AspNetUser
            {
                Id = await GetCurrentUserId(),
                UserName = newUsername,
                NormalizedUserName= newUsername.ToUpper()
            };
            _dbContext.AspNetUsers.Attach(user);
            _dbContext.Entry(user)
                .Property(x => x.UserName).IsModified = true;
            _dbContext.Entry(user)
                .Property(x => x.NormalizedUserName).IsModified = true;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private void SetupMapper()
        {
            var userMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ApplicationUserDTO>().ReverseMap();
            });
            _userMapper = new Mapper(userMapperConfig);
        }
    }
}
