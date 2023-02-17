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

namespace AuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private UsersDbContext _dbContext;

        public UserController(UserManager<ApplicationUser> userManager, UsersDbContext context)
        {
            _userManager= userManager;
            _dbContext = context;
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
            ApplicationUserDTO userDTO = new ApplicationUserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                RegistrationDate = user.RegistrationDate,
                Email = user.Email,
                Username = user.UserName,
                Age = (int)((DateTime.Now - user.DateOfBirth).TotalDays / 365.25),
                Weight = user.Weight,
                Height = user.Height,
                Gender = ((GenderType.GenderTypes)user.Gender).ToString()
            };

            return userDTO;
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
    }
}
