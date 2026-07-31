using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repo.Interfaces
{
    public interface ISessionRepo : IGenericRepo<Session>
    {

        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(
       Expression<Func<Session, bool>>? predicate = null,
       CancellationToken ct = default);

        Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default);

        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    }
}
