using AuthServer.DatabaseContext;
using AuthServer.DTOs;
using AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthServer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, HashSet<string>> _userConnections = new ();
        private UsersDbContext _dbContext;

        public ChatHub(UsersDbContext ctx)
        {
            _dbContext = ctx;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            if (!_userConnections.ContainsKey(userId))
            {
                _userConnections.Add(userId, new HashSet<string>());
            }
            _userConnections[userId].Add(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections[userId].Remove(Context.ConnectionId);
            }

            if (_userConnections[userId].Count == 0)
            {
                _userConnections.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(ChatMessageToAddDTO message)
        {
            if (message != null)
            {
                var chatMessageEntity = new ChatMessage
                {
                    ChatId = message.ChatId,
                    FromUser = message.FromUser,
                    ToUser = message.ToUser,
                    MessageText = message.MessageText,
                    SentAt = message.SentAt
                };
                _dbContext.ChatMessages.Add(chatMessageEntity);
                await _dbContext.SaveChangesAsync();
                if (_userConnections.ContainsKey(message.ToUser))
                {
                    foreach (var connection in _userConnections[message.ToUser])
                    {
                        await Clients.Client(connection).SendAsync("ReceiveMessage", chatMessageEntity);
                    }
                }
                if (_userConnections.ContainsKey(message.FromUser))
                {
                    foreach (var callerConnection in _userConnections[message.FromUser])
                    {
                        await Clients.Client(callerConnection).SendAsync("ReceiveMessage", chatMessageEntity);
                    }
                }
            }
        }

        private string GetCurrentUserId()
        {
            ClaimsPrincipal currentUser = Context.User;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
