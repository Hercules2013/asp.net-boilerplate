﻿using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using EntityFramework.DynamicFilters;

namespace Abp.EntityFramework
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class AbpDbContext : DbContext, ITransientDependency, IShouldInitialize
    {
        /// <summary>
        /// Used to get current session values.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Used to trigger entity change events.
        /// </summary>
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to GUID generator.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        /// Constructor.
        /// Uses <see cref="IAbpStartupConfiguration.DefaultNameOrConnectionString"/> as connection string.
        /// </summary>
        protected AbpDbContext()
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbCompiledModel model)
            : base(model)
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            SetNullsForInjectedProperties();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
        }

        public virtual void Initialize()
        {
            Database.Initialize(false);
            this.SetFilterScopedParameterValue(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.TenantId ?? 0);
            this.SetFilterScopedParameterValue(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.TenantId);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Filter(AbpDataFilters.SoftDelete, (ISoftDelete d) => d.IsDeleted, false);
            modelBuilder.Filter(AbpDataFilters.MustHaveTenant, (IMustHaveTenant t, int tenantId) => t.TenantId == tenantId || (int?)t.TenantId == null, 0);
            modelBuilder.Filter(AbpDataFilters.MayHaveTenant, (IMayHaveTenant t, int? tenantId) => t.TenantId == tenantId, 0);
        }

        public override int SaveChanges()
        {
            try
            {
                ApplyAbpConcepts();
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                ApplyAbpConcepts();
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        protected virtual void ApplyAbpConcepts()
        {
            var userId = GetAuditUserId();

            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        CheckAndSetId(entry);
                        SetCreationAuditProperties(entry, userId);
                        CheckAndSetTenantIdProperty(entry);
                        EntityChangeEventHelper.TriggerEntityCreatingEvent(entry.Entity);
                        EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompleted(entry.Entity);
                        break;
                    case EntityState.Modified:
                        PreventSettingCreationAuditProperties(entry);
                        CheckAndSetTenantIdProperty(entry);
                        SetModificationAuditProperties(entry, userId);

                        if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
                        {
                            SetDeletionAuditProperties(entry, userId);

                            EntityChangeEventHelper.TriggerEntityDeletingEvent(entry.Entity);
                            EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entry.Entity);
                        }
                        else
                        {
                            EntityChangeEventHelper.TriggerEntityUpdatingEvent(entry.Entity);
                            EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entry.Entity);
                        }

                        break;
                    case EntityState.Deleted:
                        PreventSettingCreationAuditProperties(entry);
                        HandleSoftDelete(entry, userId);
                        EntityChangeEventHelper.TriggerEntityDeletingEvent(entry.Entity);
                        EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entry.Entity);
                        break;
                }
            }
        }

        protected virtual void CheckAndSetId(DbEntityEntry entry)
        {
            if (entry.Entity is IEntity<Guid>)
            {
                var entity = entry.Entity as IEntity<Guid>;
                if (entity.IsTransient())
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual void CheckAndSetTenantIdProperty(DbEntityEntry entry)
        {
            if (entry.Entity is IMustHaveTenant)
            {
                CheckAndSetMustHaveTenant(entry);
            }
        }

        protected virtual void CheckAndSetMustHaveTenant(DbEntityEntry entry)
        {
            var entity = entry.Cast<IMustHaveTenant>().Entity;
            var currentTenantId = CurrentUnitOfWorkProvider.Current.GetTenantId();

            if (entity.TenantId == 0)
            {
                if (currentTenantId != null)
                {
                    entity.TenantId = currentTenantId.Value;
                }
                else
                {
                    throw new AbpException("Can not set TenantId to 0 for IMustHaveTenant entities!");
                }
            }
        }

        protected virtual void SetCreationAuditProperties(DbEntityEntry entry, long? userId)
        {
            if (entry.Entity is IHasCreationTime)
            {
                entry.Cast<IHasCreationTime>().Entity.CreationTime = Clock.Now;
            }

            if (userId.HasValue && entry.Entity is ICreationAudited)
            {
                var entity = entry.Cast<ICreationAudited>().Entity;
                if (entity.CreatorUserId == null)
                {
                    entity.CreatorUserId = userId;
                }
            }
        }

        protected virtual void PreventSettingCreationAuditProperties(DbEntityEntry entry)
        {
            //TODO@Halil: Implement this when tested well (Issue #49)
            //if (entry.Entity is IHasCreationTime && entry.Cast<IHasCreationTime>().Property(e => e.CreationTime).IsModified)
            //{
            //    throw new DbEntityValidationException(string.Format("Can not change CreationTime on a modified entity {0}", entry.Entity.GetType().FullName));
            //}

            //if (entry.Entity is ICreationAudited && entry.Cast<ICreationAudited>().Property(e => e.CreatorUserId).IsModified)
            //{
            //    throw new DbEntityValidationException(string.Format("Can not change CreatorUserId on a modified entity {0}", entry.Entity.GetType().FullName));
            //}
        }

        protected virtual void SetModificationAuditProperties(DbEntityEntry entry, long? userId)
        {
            if (entry.Entity is IHasModificationTime)
            {
                entry.Cast<IHasModificationTime>().Entity.LastModificationTime = Clock.Now;
            }

            if (userId.HasValue && entry.Entity is IModificationAudited)
            {
                var entity = entry.Cast<IModificationAudited>().Entity;
                entity.LastModifierUserId = userId;
            }
        }

        protected virtual void HandleSoftDelete(DbEntityEntry entry, long? userId)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            var softDeleteEntry = entry.Cast<ISoftDelete>();

            softDeleteEntry.State = EntityState.Unchanged;
            softDeleteEntry.Entity.IsDeleted = true;

            SetDeletionAuditProperties(entry, userId);
        }

        protected virtual void SetDeletionAuditProperties(DbEntityEntry entry, long? userId)
        {
            if (entry.Entity is IHasDeletionTime)
            {
                entry.Cast<IHasDeletionTime>().Entity.DeletionTime = Clock.Now;
            }

            if (userId.HasValue && entry.Entity is IDeletionAudited)
            {
                entry.Cast<IDeletionAudited>().Entity.DeleterUserId = AbpSession.UserId;
            }
        }

        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            Logger.Error("There are some validation errors while saving changes in EntityFramework:");
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }

        protected virtual long? GetAuditUserId()
        {
            if (AbpSession.UserId.HasValue &&
                CurrentUnitOfWorkProvider != null &&
                CurrentUnitOfWorkProvider.Current != null &&
                CurrentUnitOfWorkProvider.Current.GetTenantId() == AbpSession.TenantId)
            {
                return AbpSession.UserId;
            }

            return null;
        }
    }
}
