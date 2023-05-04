using System;
using System.Collections.Generic;

namespace AuthServer.Models
{
    public partial class DoctorRequest
    {
        public string UserId { get; set; } = null!;
        public string PassportPicture { get; set; } = null!;
        public string DiplomaPicture { get; set; } = null!;

        public virtual AspNetUser User { get; set; } = null!;
    }
}
