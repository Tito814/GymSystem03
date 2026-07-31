using GymManagementDAL.Repositories.Interfaces;
using GymSystem;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Classes;
using Microsoft.EntityFrameworkCore;

namespace GymManagementDAL.Repositories.Classes
{
    public class BookingRepository : GenericRepo<Booking>, IBookingRepository

    {
        private readonly GymAppContext _dbContext;

        public BookingRepository(GymAppContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking().Include(b => b.member).Where(b => b.sessionId == sessionId).ToListAsync(ct);


    }
}
