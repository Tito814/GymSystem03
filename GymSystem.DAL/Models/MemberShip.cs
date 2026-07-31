using GymSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class MemberShip : BaseEntity
    {
        public Member member { get; set; }
        public int memberId { get; set; }

        public Plan plan { get; set; }
        public int planId { get; set; }

        // Startdate is equal createdAt
        public DateTime EndDate { get; set; }


    }
}
