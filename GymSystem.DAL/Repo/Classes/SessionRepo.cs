using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repo.Classes
{
    public class SessionRepo : GenericRepo<Session>, ISessionRepo
    {
        private readonly GymAppContext _dbcontext;
        public SessionRepo(GymAppContext context) : base(context)
        {
            _dbcontext = context;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Session> session = _dbcontext.Sessions.AsNoTracking()
                .Include(s => s.trainer)
                .Include(s => s.category);

            if (predicate != null)
                session = session.Where(predicate);

            return await session.ToListAsync(ct);

        }

        public Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
            => _dbcontext.Bookings.AsNoTracking().CountAsync(s => s.sessionId == sessionId, ct);


        public Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
            => _dbcontext.Sessions.AsNoTracking().Include(s => s.trainer).Include(s => s.category).FirstOrDefaultAsync(s => s.Id == sessionId, ct);

    }
}
