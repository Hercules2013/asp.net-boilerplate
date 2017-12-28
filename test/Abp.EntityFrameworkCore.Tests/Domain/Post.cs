﻿using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.EntityHistory;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    [HistoryTracked]
    public class Post : AuditedEntity<Guid>, ISoftDelete, IMayHaveTenant
    {
        [Required]
        public Blog Blog { get; set; }

        public int BlogId { get; set; }

        public string Title { get; set; }

        [DisableHistoryTracking]
        public string Body { get; set; }

        public bool IsDeleted { get; set; }

        public int? TenantId { get; set; }

        public Post()
        {
            Id = Guid.NewGuid();
        }

        public Post(Blog blog, string title, string body)
            : this()
        {
            Blog = blog;
            Title = title;
            Body = body;
        }
    }
}