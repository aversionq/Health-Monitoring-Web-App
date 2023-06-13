using AuthServer.Models;
using AuthServer.Roles;
using AuthServer.ViewModels;
using AuthServer.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        // It's done this way because ProfilePicture is Nullable, cant wipe DB right now. Refactor soon.
        private readonly string _defaultProfilePicture = @"https://cdn-icons-png.flaticon.com/512/1430/1430453.png";

        public AuthController(RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
        {
            var userExistsEmail = await _userManager.FindByEmailAsync(model.Email);
            var userExistsUsername = await _userManager.FindByNameAsync(model.Username);
            if (userExistsEmail != null && userExistsUsername != null)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "These Email and Username were already taken",
                    ErrorCode = 600
                });
            }
            if (userExistsEmail != null)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "This Email was already taken",
                    ErrorCode = 610
                });
            }
            if (userExistsUsername != null)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "This Username was already taken",
                    ErrorCode = 620
                });
            }
            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Username,
                //FirstName = model.FirstName,
                //LastName = model.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                //DateOfBirth = model.DateOfBirth.Date,
                RegistrationDate = DateTime.Now,
                //Weight = model.Weight,
                //Height = model.Height,
                //Gender = model.Gender,
                IPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                ProfilePicture = _defaultProfilePicture
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse
                    {
                        ErrorDescription = "An error occured while creating a user",
                        ErrorCode = 630
                    });
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.DefaultUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.DefaultUser));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Doctor))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.DefaultUser))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.DefaultUser);
            }

            var loginResult = await Login(new LoginViewModel
            {
                Username = model.Username,
                Password = model.Password
            }) as OkObjectResult;

            return Ok(loginResult.Value);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username) ??
                await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
                    };

                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256),
                        expires: DateTime.Now.AddDays(30)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
                return Unauthorized(new ErrorResponse
                {
                    ErrorDescription = "Wrong Password",
                    ErrorCode = 640
                });
            }
            return Unauthorized(new ErrorResponse
            {
                ErrorDescription = "Wrong Email or Username",
                ErrorCode = 650
            });
        }
    }
}
