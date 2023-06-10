namespace AuthServer.DTOs
{
    public class ChatMessageToAddDTO
    {
        public Guid ChatId { get; set; }
        public string FromUser { get; set; } = null!;
        public string ToUser { get; set; } = null!;
        public string MessageText { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}
