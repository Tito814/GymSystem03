using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Booking : BaseEntity
    {
        public Session session { get; set; }
        public Member member { get; set; }
        public int sessionId { get; set; }
        public int memberId { get; set; }

        // booking date is equal createdAt

        public bool IsAttended { get; set; }
    }

}
