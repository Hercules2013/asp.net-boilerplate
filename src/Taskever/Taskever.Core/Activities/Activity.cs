﻿using System;
using Abp.Domain.Entities;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Activities
{
    public abstract class Activity : Entity<long>
    {
        public virtual ActivityType ActivityType { get; set; }

        public virtual DateTime CreationTime { get; set; }

        protected Activity()
        {
            CreationTime = DateTime.Now;
        }

        public abstract AbpUser[] GetActors();

        public abstract AbpUser[] GetRelatedUsers();
    }
}
