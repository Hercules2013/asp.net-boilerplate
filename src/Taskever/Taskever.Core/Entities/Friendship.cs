﻿using Abp.Entities;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Entities.Utils;

namespace Taskever.Entities
{
    public class Friendship : Entity, IHasTenant
    {
        /// <summary>
        /// The tenant account which this entity is belong to.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        public virtual User User { get; set; }
        
        public virtual User Friend { get; set; }

        public virtual FriendshipStatus Status { get; set; }
    }
}
