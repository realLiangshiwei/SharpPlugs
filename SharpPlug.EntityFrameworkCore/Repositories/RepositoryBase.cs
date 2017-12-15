using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using SharpPlug.EntityFrameworkCore.Entity;

// ReSharper disable once CheckNamespace
namespace SharpPlug.EntityFrameworkCore.RepositoriesBase
{
    public class Repository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly DbContext _dbcontext;

        public Repository(DbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public virtual DbSet<TEntity> Table => _dbcontext.Set<TEntity>();


        public virtual IDbContextTransaction Transaction => _dbcontext.Database.BeginTransaction();


        public virtual async Task<IDbContextTransaction> TransactionAsync()
        {
            return await _dbcontext.Database.BeginTransactionAsync();
        }


        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Table.AsQueryable();

            if (propertySelectors == null || propertySelectors.Length <= 0) return query;
            foreach (var propertySelector in propertySelectors)
            {
                query = EntityFrameworkQueryableExtensions.Include(query, propertySelector);
            }
            return query;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await EntityFrameworkQueryableExtensions.ToListAsync(GetAll());
        }

        public List<TEntity> GetAllList()
        {
            return Enumerable.ToList(GetAll());
        }

        public int Count()
        {
            return Queryable.Count(GetAll());
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable.Count(GetAll(), predicate);
        }

        public long LongCount()
        {
            return Queryable.LongCount(GetAll());
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable.LongCount(GetAll(), predicate);
        }

        public async Task<int> CountAsync()
        {
            return await EntityFrameworkQueryableExtensions.CountAsync(GetAll());
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await EntityFrameworkQueryableExtensions.CountAsync(GetAll(), predicate);
        }

        public async Task<long> LongCountAsync()
        {
            return await EntityFrameworkQueryableExtensions.LongCountAsync(GetAll());
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await EntityFrameworkQueryableExtensions.LongCountAsync(GetAll(), predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await EntityFrameworkQueryableExtensions.SingleAsync(GetAll(), predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(TKey id)
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(GetAll(), o => o.Id.Equals(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(GetAll(), predicate);
        }

        public TEntity FirstOrDefault(TKey id)
        {
            return Queryable.FirstOrDefault(GetAll(), o => o.Id.Equals(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable.FirstOrDefault(GetAll(), predicate);
        }


        public TEntity AddOrUpdate(TEntity entity)
        {
            if (EntityExtensions.HasId(entity))
            {
                Attach(entity);
                Table.Update(entity);
            }
            else
            {
                Table.Add(entity);
            }

            return entity;
        }

        public async Task<TEntity> AddOrUpdateAsync(TEntity entity)
        {
            return await Task.FromResult(AddOrUpdate(entity));
        }

        public async Task<TKey> AddOrUpdateAndGetIdAsync(TEntity entity)
        {
            if (EntityExtensions.HasId(entity))
                Table.Update(entity);
            else
                Table.Add(entity);
            await _dbcontext.SaveChangesAsync();
            return entity.Id;
        }

        public TKey AddOrUpdateAndGetId(TEntity entity)
        {
            if (EntityExtensions.HasId(entity))
                Table.Update(entity);
            else
                Table.Add(entity);
            _dbcontext.SaveChanges();
            return entity.Id;
        }


        protected virtual void Attach(TEntity entity)
        {
            var entry = Enumerable.FirstOrDefault<EntityEntry>(_dbcontext.ChangeTracker.Entries(), ent => ent.Entity == entity);
            if (entry != null)
                return;
            Table.Attach(entity);
        }


    }
}
