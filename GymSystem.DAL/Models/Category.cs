using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }

        public ICollection<Session> sessions { get; set; }
    }
}
