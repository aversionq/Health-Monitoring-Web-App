using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            ChatMessageFromUserNavigations = new HashSet<ChatMessage>();
            ChatMessageToUserNavigations = new HashSet<ChatMessage>();
            Chats = new HashSet<Chat>();
            DoctorPatientDoctors = new HashSet<DoctorPatient>();
            DoctorPatientUsers = new HashSet<DoctorPatient>();
            UserChats = new HashSet<UserChat>();
            Roles = new HashSet<AspNetRole>();
        }

        public string Id { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public int? Gender { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string Ipaddress { get; set; } = null!;
        public string? ProfilePicture { get; set; }

        public virtual DoctorRequest? DoctorRequest { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<ChatMessage> ChatMessageFromUserNavigations { get; set; }
        public virtual ICollection<ChatMessage> ChatMessageToUserNavigations { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<DoctorPatient> DoctorPatientDoctors { get; set; }
        public virtual ICollection<DoctorPatient> DoctorPatientUsers { get; set; }
        public virtual ICollection<UserChat> UserChats { get; set; }

        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
