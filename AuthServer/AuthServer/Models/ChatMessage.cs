using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string FromUser { get; set; } = null!;
        public string ToUser { get; set; } = null!;
        public string MessageText { get; set; } = null!;
        public DateTime SentAt { get; set; }

        public virtual Chat Chat { get; set; } = null!;
        public virtual AspNetUser FromUserNavigation { get; set; } = null!;
        public virtual AspNetUser ToUserNavigation { get; set; } = null!;
    }
}
