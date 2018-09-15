using Microsoft.EntityFrameworkCore;
using SharpPlug.EntityFrameworkCore.Entity;
using SharpPlug.EntityFrameworkCore.RepositoriesBase;

namespace SharpPlug.EntityFrameworkCore.Repositories
{
    public class Repository<TEntity> : Repository<TEntity, int> where TEntity : class, IEntity<int>
    {
        public Repository(DbContext dbcontext) : base(dbcontext)
        {
        }
    }
}
