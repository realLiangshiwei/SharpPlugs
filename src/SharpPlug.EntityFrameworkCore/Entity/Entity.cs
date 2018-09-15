using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPlug.EntityFrameworkCore.Entity
{
    public class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}
