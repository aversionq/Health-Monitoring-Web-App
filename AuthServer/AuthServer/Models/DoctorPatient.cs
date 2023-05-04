using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class DoctorPatient
    {
        public string DoctorId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public Guid Id { get; set; }

        public virtual AspNetUser Doctor { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
