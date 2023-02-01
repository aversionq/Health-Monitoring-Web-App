using AuthServer.DTOs;
using AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace AuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager= userManager;
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
    }
}
