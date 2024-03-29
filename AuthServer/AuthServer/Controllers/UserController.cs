﻿using AuthServer.DatabaseContext;
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
                Gender = user.Gender is null ? null : ((GenderType.GenderTypes)user.Gender).ToString(),
                ProfilePicture = user.ProfilePicture,
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
                return Ok(new
                {
                    updValue = metrics.Value
                });
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
                return Ok(new
                {
                    updValue = metrics.Value
                });
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
        public async Task<ActionResult> EditUsername([FromBody] StringDataChange sdc)
        {
            if (await _userManager.FindByNameAsync(sdc.Value) != null)
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
                UserName = sdc.Value,
                NormalizedUserName= sdc.Value.ToUpper()
            };
            _dbContext.AspNetUsers.Attach(user);
            _dbContext.Entry(user)
                .Property(x => x.UserName).IsModified = true;
            _dbContext.Entry(user)
                .Property(x => x.NormalizedUserName).IsModified = true;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                updValue = sdc.Value
            });
        }

        [HttpPatch]
        [Route("changeFirstName")]
        public async Task<ActionResult> ChangeUserFirstName([FromBody] StringDataChange sdc)
        {
            try
            {
                var user = new AspNetUser
                {
                    Id = await GetCurrentUserId(),
                    FirstName = sdc.Value,
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user)
                    .Property(x => x.FirstName)
                    .IsModified = true;
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    updValue = sdc.Value
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to change user first name",
                    ErrorCode = 680
                });
            }
        }

        [HttpPatch]
        [Route("changeLastName")]
        public async Task<ActionResult> ChangeUserLastName([FromBody] StringDataChange sdc)
        {
            try
            {
                var user = new AspNetUser
                {
                    Id = await GetCurrentUserId(),
                    LastName = sdc.Value,
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user)
                    .Property(x => x.LastName)
                    .IsModified = true;
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    updValue = sdc.Value
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to change user last name",
                    ErrorCode = 690
                });
            }
        }

        [HttpPatch]
        [Route("changeUserBirthday")]
        public async Task<ActionResult> ChangeUserBirthday([FromBody] DatetimeDataChange ddc)
        {
            try
            {
                var user = new AspNetUser
                {
                    Id = await GetCurrentUserId(),
                    DateOfBirth = ddc.Value,
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user)
                    .Property(x => x.DateOfBirth)
                    .IsModified = true;
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    updValue = ddc.Value
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to change user birthday",
                    ErrorCode = 6900
                });
            }
        }

        [HttpPatch]
        [Route("changeUserGender")]
        public async Task<ActionResult> ChangeUserGender([FromBody] StringDataChange sdc)
        {
            try
            {
                GenderType.GenderTypes parsedGender;
                bool result = Enum.TryParse(sdc.Value, out parsedGender);
                if (result)
                {
                    var user = new AspNetUser
                    {
                        Id = await GetCurrentUserId(),
                        Gender = (int)parsedGender,
                    };
                    _dbContext.AspNetUsers.Attach(user);
                    _dbContext.Entry(user)
                        .Property(x => x.Gender)
                        .IsModified = true;
                    await _dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        updValue = sdc.Value
                    });
                }
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Wrong gender enum type",
                    ErrorCode = 95000
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Not able to change user gender",
                    ErrorCode = 6900
                });
            }
        }

        [HttpPatch]
        [Route("changeUserProfilePicture")]
        public async Task<ActionResult> ChangeUserProfilePicture([FromForm] PictureUpload pfp)
        {
            try
            {
                var userId = await GetCurrentUserId();
                var pfpLink = await _pictureUploadService.UploadImage(pfp);
                var user = new AspNetUser
                {
                    Id = userId,
                    ProfilePicture = pfpLink
                };
                _dbContext.AspNetUsers.Attach(user);
                _dbContext.Entry(user)
                    .Property(x => x.ProfilePicture)
                    .IsModified = true;
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    updValue = pfpLink
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to update user's profile picture",
                    ErrorCode = 287000
                });
            }
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
