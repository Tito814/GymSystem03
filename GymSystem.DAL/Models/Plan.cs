using GymSystem.DAL.Models;

namespace GymSystem.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MemberShip> members { get; set; }

    }
}
