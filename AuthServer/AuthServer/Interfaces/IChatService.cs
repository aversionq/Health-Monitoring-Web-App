using AuthServer.DTOs;

namespace AuthServer.Interfaces
{
    public interface IChatService
    {
        public Task<IEnumerable<ChatDTO>> GetUserChats(string userId);
        public Task<IEnumerable<ChatMessageDTO>> GetUserChatMessages(Guid chatId);
        public Task AddChat(ChatToAddDTO chatToAddDTO);
        public Task AddMessage(ChatMessageToAddDTO chatMessageDTO);
        public Task<Guid> GetChatIdByUsers(string userId, string doctorId);
    }
}
