using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPlug.EntityFrameworkCore.Entity
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }


    public static class EntityExtensions
    {
        public static bool HasId<TKey>(this IEntity<TKey> entity)
        {
            if (EqualityComparer<TKey>.Default.Equals(entity.Id, default(TKey)))
            {
                return true;
            }

            if (typeof(TKey) == typeof(long))
            {
                return Convert.ToInt64(entity.Id) <= 0;
            }
            if (typeof(TKey) == typeof(int))
            {
                return Convert.ToInt32(entity.Id) <= 0;
            }
            return false;
        }
    }
}
