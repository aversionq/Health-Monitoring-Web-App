namespace AuthServer.DTOs
{
    public class ChatToAddDTO
    {
        public string LastMessageText { get; set; } = null!;
        public DateTime LastMessageDate { get; set; }
        public string FromUser { get; set; } = null!;
        public string ToUser { get; set; } = null!;
    }
}
