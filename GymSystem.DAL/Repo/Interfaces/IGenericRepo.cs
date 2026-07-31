using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repo.Interfaces
{
    public interface IGenericRepo<TEntity> where TEntity : BaseEntity, new()
    {
        // Get all
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        // Get By Id
        Task<TEntity> GetByIDAsync(int id, CancellationToken ct = default);
        // Add
        void AddAsync(TEntity entity);
        // Update
        void UpdateAsync(TEntity entity);
        // Delete
        void DeleteAsync(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken ct = default);
    }
}
