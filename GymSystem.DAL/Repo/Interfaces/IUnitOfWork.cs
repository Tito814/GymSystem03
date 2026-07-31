using GymManagementDAL.Repositories.Interfaces;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repo.Interfaces
{
    public interface IUnitOfWork
    {
        public IMembershipRepository MembershipRepository { get; }
        public ISessionRepo SessionRepository { get; }
        public IBookingRepository BookingRepository { get; }
        IGenericRepo<TEntity> GetRepo<TEntity>() where TEntity : BaseEntity, new();
        Task<int> Completed(CancellationToken ct = default);

    }
}
