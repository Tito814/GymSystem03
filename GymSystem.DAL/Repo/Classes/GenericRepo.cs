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
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : BaseEntity, new()
    {
        // DB Connection
        private readonly GymAppContext _context;

        public GenericRepo(GymAppContext context)
        {
            _context = context;
        }
        public async void AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);

        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default)
            => _context.Set<TEntity>().AsNoTracking().AnyAsync(expression, ct);

        public async void DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
         }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();
            return await query.FirstOrDefaultAsync(expression, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity> GetByIDAsync(int id, CancellationToken ct = default)
            => await _context.Set<TEntity>().FindAsync(id, ct);

        public async void UpdateAsync(TEntity entity)
        {
             _context.Set<TEntity>().Update(entity);
        }
    }
}
