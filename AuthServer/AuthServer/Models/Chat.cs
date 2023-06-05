using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class Chat
    {
        public Chat()
        {
            ChatMessages = new HashSet<ChatMessage>();
            UserChats = new HashSet<UserChat>();
        }

        public Guid Id { get; set; }
        public string LastMessageText { get; set; } = null!;
        public DateTime LastMessageDate { get; set; }
        public string FromUser { get; set; } = null!;

        public virtual AspNetUser FromUserNavigation { get; set; } = null!;
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
