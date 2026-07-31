using GymSystem.Config;
using GymSystem.DAL.Models;
using GymSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem
{
    public class GymAppContext : DbContext
    {
        public GymAppContext(DbContextOptions<GymAppContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}
