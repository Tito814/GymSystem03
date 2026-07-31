using GymSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Member : User
    {
        public string? photo { get; set; }

        public HealthRecord healthrecord { get; set; } = default!;

        public ICollection<MemberShip> plans { get; set; }

        public ICollection<Booking> bookings { get; set; }

    }
    
}
