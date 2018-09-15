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
                query = query.Include(propertySelector);
            }
            return query;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().CountAsync(predicate);
        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().LongCountAsync(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(TKey id)
        {
            return await GetAll().FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefault(TKey id)
        {
            return GetAll().FirstOrDefault(o => o.Id.Equals(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }


        public TEntity AddOrUpdate(TEntity entity)
        {
            if (entity.HasId())
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
            if (entity.HasId())
                Table.Update(entity);
            else
                Table.Add(entity);
            await _dbcontext.SaveChangesAsync();
            return entity.Id;
        }

        public TKey AddOrUpdateAndGetId(TEntity entity)
        {
            if (entity.HasId())
                Table.Update(entity);
            else
                Table.Add(entity);
            _dbcontext.SaveChanges();
            return entity.Id;
        }


        protected virtual void Attach(TEntity entity)
        {
            var entry = _dbcontext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
                return;
            Table.Attach(entity);
        }


    }
}
