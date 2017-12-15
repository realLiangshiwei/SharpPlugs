using System;

namespace SharpPlug.EntityFrameworkCore.Entity
{
    public class Entity : IEntity<int>
    {
        public int Id { get; set; }
    }
}
