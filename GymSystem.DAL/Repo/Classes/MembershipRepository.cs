
using GymManagementDAL.Repositories.Interfaces;
using GymSystem;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Classes;
using GymSystem.DAL.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Classes
{
	public class MembershipRepository : GenericRepo<MemberShip>, IMembershipRepository
	{
		private readonly GymAppContext _dbContext;

		public MembershipRepository(GymAppContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

        public async Task<List<MemberShip>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? predicate = null,
           CancellationToken ct = default)
        {
            IQueryable<MemberShip> query = _dbContext.MemberShips.AsNoTracking().Include(m => m.plan).Include(m => m.member);

            if (predicate is not null) query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

	}
}
