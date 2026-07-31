using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repo.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymAppContext _context;
        private readonly Dictionary<string, object> _repos = new Dictionary<string, object>();

        public ISessionRepo SessionRepository { get; }

        public UnitOfWork(GymAppContext context, ISessionRepo sessionRepository)
        {
            _context = context;
            SessionRepository = sessionRepository;
        }
        public IGenericRepo<TEntity> GetRepo<TEntity>() where TEntity : BaseEntity, new()
        {
            // Get Name of the type of the entity
            var TypeName = typeof(TEntity).Name;

            // Check if the repo of this type is already created
            if (_repos.TryGetValue(TypeName, out object? repo))
                return (IGenericRepo<TEntity>)repo;

            else
            {
                // Create the repo of this type and add it to the dictionary
                var newRepo = new GenericRepo<TEntity>(_context);
                _repos.Add(TypeName, newRepo);
                return newRepo;
            }
        }

        public async Task<int> Completed(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

    }
}
