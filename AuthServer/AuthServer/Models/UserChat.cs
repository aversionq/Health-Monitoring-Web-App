using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class UserChat
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string UserId { get; set; } = null!;

        public virtual Chat Chat { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
