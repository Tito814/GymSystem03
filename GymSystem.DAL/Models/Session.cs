using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Trainer trainer { get; set; } = default!;
        public int trainerId { get; set; }


        public Category category { get; set; } = default!;
        public int categoryId { get; set; }

        public ICollection<Booking> sessionmember { get; set; }
    }
}
