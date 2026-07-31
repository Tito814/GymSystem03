
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IBookingRepository : IGenericRepo<Booking>
	{
        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default);

    }
}
