﻿namespace AuthServer.DTOs
{
    public class ChatPartnerDTO
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
