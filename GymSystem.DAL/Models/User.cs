using GymSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Models
{
    public abstract class User : BaseEntity
    {
        public string Name { get; set; } = default;
        public string Email { get; set; } = default;
        public string Phone { get; set; } = default;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public Address address { get; set; }
    }
    public class Address
    {
        public int BuildingNumber { get; set; } = default;
        public string City { get; set; } = default;
        public string Street { get; set; } = default;
    }
}
