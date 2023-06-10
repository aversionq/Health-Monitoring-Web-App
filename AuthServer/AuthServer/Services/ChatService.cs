using AuthServer.DatabaseContext;
using AuthServer.DTOs;
using AuthServer.Interfaces;
using AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Services
{
    public class ChatService : IChatService
    {
        private readonly UsersDbContext _dbContext;
        private Mapper _messageMapper;

        public ChatService(UsersDbContext ctx)
        {
            _dbContext = ctx;
            SetupMappers();
        }

        public async Task AddChat(ChatToAddDTO chatToAddDTO)
        {
            var chatId = Guid.NewGuid();

            var newChat = new Chat
            {
                Id = chatId,
                LastMessageText = chatToAddDTO.LastMessageText,
                LastMessageDate = chatToAddDTO.LastMessageDate,
                FromUser = chatToAddDTO.FromUser
            };
            _dbContext.Chats.Add(newChat);
            await _dbContext.SaveChangesAsync();

            _dbContext.UserChats.Add(new UserChat
            {
                ChatId = chatId,
                UserId = chatToAddDTO.FromUser
            });
            _dbContext.UserChats.Add(new UserChat
            {
                ChatId = chatId,
                UserId = chatToAddDTO.ToUser
            });
            await _dbContext.SaveChangesAsync();

            var newMessage = new ChatMessageToAddDTO
            {
                ChatId = chatId,
                FromUser = chatToAddDTO.FromUser,
                ToUser = chatToAddDTO.ToUser,
                MessageText = chatToAddDTO.LastMessageText,
                SentAt = chatToAddDTO.LastMessageDate
            };
            await AddMessage(newMessage);
        }

        public async Task AddMessage(ChatMessageToAddDTO chatMessageDTO)
        {
            var chatMessageEntity = new ChatMessage
            {
                ChatId = chatMessageDTO.ChatId,
                FromUser = chatMessageDTO.FromUser,
                ToUser = chatMessageDTO.ToUser,
                MessageText = chatMessageDTO.MessageText,
                SentAt = chatMessageDTO.SentAt
            };
            _dbContext.ChatMessages.Add(chatMessageEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid> GetChatIdByUsers(string userId, string doctorId)
        {
            var userChats = await _dbContext.UserChats
                .Where(x => x.UserId == userId)
                .Select(x => x.ChatId)
                .ToListAsync();
            var doctorChats = await _dbContext.UserChats
                .Where(x => x.UserId == doctorId)
                .Select(x => x.ChatId)
                .ToListAsync();

            var chatId = userChats.Intersect(doctorChats).FirstOrDefault();
            return chatId;
        }

        public async Task<IEnumerable<ChatMessageDTO>> GetUserChatMessages(Guid chatId)
        {
            var messages = await _dbContext.ChatMessages
                .Where(x => x.ChatId == chatId)
                .OrderBy(x => x.SentAt)
                .ToListAsync();
            var messagesDTO = _messageMapper.Map<List<ChatMessage>, List<ChatMessageDTO>>(messages);
            return messagesDTO;
        }

        public async Task<IEnumerable<ChatDTO>> GetUserChats(string userId)
        {
            var chatIds = await _dbContext.UserChats
                .Where(x => x.UserId == userId)
                .Select(x => x.ChatId)
                .ToListAsync();

            //var otherUsersChatDict = new Dictionary<Guid, string>();
            var userChatsDTO = new List<ChatDTO>();
            foreach (var chatId in chatIds)
            {
                var otherUserId = await _dbContext.UserChats
                    .Where(x => x.ChatId == chatId && x.UserId != userId)
                    .Select(x => x.UserId)
                    .FirstOrDefaultAsync();
                var otherUserUsername = await _dbContext.AspNetUsers
                    .Where(x => x.Id == otherUserId)
                    .Select(x => x.UserName)
                    .FirstOrDefaultAsync();
                var chatInfo = await _dbContext.Chats
                    .Where(x => x.Id == chatId)
                    .FirstOrDefaultAsync();
                var fromUsername = await _dbContext.AspNetUsers
                    .Where(x => x.Id == chatInfo.FromUser)
                    .Select(x => x.UserName)
                    .FirstOrDefaultAsync();
                var chatDTO = new ChatDTO
                {
                    Id = chatId,
                    LastMessageText = chatInfo.LastMessageText,
                    LastMessageDate = chatInfo.LastMessageDate,
                    FromUsername = fromUsername,
                    OtherUserPicture = null,
                    OtherUserId = otherUserId,
                    OtherUserUsername = otherUserUsername
                };
                userChatsDTO.Add(chatDTO);
            }

            return userChatsDTO;
        }

        private void SetupMappers()
        {
            var messageConfig = new MapperConfiguration(o => o.CreateMap<ChatMessage, ChatMessageDTO>().ReverseMap());
            _messageMapper = new Mapper(messageConfig);
        }
    }
}
