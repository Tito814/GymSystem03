using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IMembershipRepository : IGenericRepo<MemberShip>
	{
        Task<List<MemberShip>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? predicate = null,CancellationToken ct = default);
    }
}
