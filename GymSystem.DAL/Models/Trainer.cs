using GymSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Trainer : User
    {
        public Specialty Specialty { get; set; }

        public ICollection<Session> sessions { get; set; }
    }
}
