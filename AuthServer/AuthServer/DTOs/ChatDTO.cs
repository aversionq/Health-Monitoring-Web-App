namespace AuthServer.DTOs
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        public string LastMessageText { get; set; } = null!;
        public DateTime LastMessageDate { get; set; }
        public string FromUsername { get; set; }
        public string OtherUserPicture { get; set; }
        public string OtherUserId { get; set; }
        public string OtherUserUsername { get; set; }
    }
}
