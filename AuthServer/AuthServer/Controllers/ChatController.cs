using AuthServer.DTOs;
using AuthServer.Interfaces;
using AuthServer.Models;
using AuthServer.ResponseModels;
using AuthServer.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace AuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatService _chatService;
        private UserManager<ApplicationUser> _userManager;

        public ChatController(IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("getUserChats")]
        public async Task<ActionResult<List<ChatDTO>>> GetUserChats()
        {
            var userId = GetCurrentUserId();
            var chats = await _chatService.GetUserChats(userId);
            return Ok(chats);
        }

        [HttpGet]
        [Route("getChatByUserIds")]
        public async Task<ActionResult<Guid>> GetChatByUserIds(string userId, string doctorId)
        {
            try
            {
                var chatId = await _chatService.GetChatIdByUsers(userId, doctorId);
                if (chatId == default)
                {
                    throw new Exception();
                }
                return Ok(chatId);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Unable to get chat for these users",
                    ErrorCode = 152000
                });
            }
        }

        [HttpGet]
        [Route("getChatMessages")]
        public async Task<ActionResult<List<ChatMessageDTO>>> GetChatMessages(Guid chatId)
        {
            var messages = await _chatService.GetUserChatMessages(chatId);
            return Ok(messages);
        }

        [HttpGet]
        [Route("getChatPartnerInfo")]
        public async Task<ActionResult<ChatPartnerDTO>> GetChatPartnerInfo(string userId)
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
            ChatPartnerDTO chatPartner = new ChatPartnerDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRole,
                Username = user.UserName,
                ProfilePicture = user.ProfilePicture
            };

            return chatPartner;
        }

        [HttpPost]
        [Route("addChat")]
        public async Task<ActionResult> AddNewChat(ChatToAddDTO chat)
        {
            await _chatService.AddChat(chat);
            return Ok();
        }

        [HttpPost]
        [Route("addMessageToChat")]
        public async Task<ActionResult> AddMessageToChat(ChatMessageToAddDTO message)
        {
            await _chatService.AddMessage(message);
            return Ok();
        }

        private string GetCurrentUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
