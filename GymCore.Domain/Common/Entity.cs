using System;
using System.Collections.Generic;
using System.Text;

namespace GymCore.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt {  get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        protected void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
