using Common.Utilities;
using Data.Contracts;
using Entity.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Entity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Data.Reprositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly ApplicationDbContext dbContext;
        public DbSet<TEntity> Entities { get; }
        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Repository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            Entities = dbContext.Set<TEntity>(); // City => Cities
        }

        #region Async Method

        public virtual async Task<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return await Entities.FindAsync(ids, cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            var entityString = JsonConvert.SerializeObject(entity);
            var userId = _httpContextAccessor.HttpContext?.User.Identity?.GetUserId<int>() ?? 0;
            var audit = new Audit
            {
                CreatedAt = DateTimeOffset.Now,
                Model = entity.GetType().Name,
                Method = "Add",
                OldValue = "",
                NewValue = entityString,
                UserId = userId
            };
            await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.AddAsync(audit, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
            bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            var entityString = JsonConvert.SerializeObject(entity);
            var entry = dbContext.Entry(entity);
            var oldEntity = entry.OriginalValues.Clone();
            var oldEntityString = JsonConvert.SerializeObject(oldEntity.Properties.ToDictionary(i => i.Name, i => oldEntity[i]));
            var userId = _httpContextAccessor.HttpContext?.User.Identity?.GetUserId<int>() ?? 0;
            var audit = new Audit
            {
                CreatedAt = DateTimeOffset.Now,
                Model = entity.GetType().Name,
                Method = "Update",
                OldValue = oldEntityString, 
                NewValue = entityString,
                UserId = userId
            };
            Entities.Update(entity);
            await dbContext.AddAsync(audit, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
            bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.UpdateRange(entities);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            var userId = _httpContextAccessor.HttpContext?.User.Identity?.GetUserId<int>() ?? 0;
            var audit = new Audit
            {
                CreatedAt = DateTimeOffset.Now,
                Model = entity.GetType().Name,
                Method = "Delete",
                OldValue = "", 
                NewValue = "",
                UserId = userId
            };
            Entities.Remove(entity);
            await dbContext.AddAsync(audit, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken,
            bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.RemoveRange(entities);
            if (saveNow)
                await dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Sync Methods

        public virtual TEntity GetById(params object[] ids)
        {
            return Entities.Find(ids);
        }

        public virtual void Add(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            Entities.Add(entity);
            if (saveNow)
                dbContext.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.AddRange(entities);
            if (saveNow)
                dbContext.SaveChanges();
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            Entities.Update(entity);
            dbContext.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.UpdateRange(entities);
            if (saveNow)
                dbContext.SaveChanges();
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            Entities.Remove(entity);
            if (saveNow)
                dbContext.SaveChanges();
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.RemoveRange(entities);
            if (saveNow)
                dbContext.SaveChanges();
        }

        #endregion

        #region Attach & Detach

        public virtual void Detach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            var entry = dbContext.Entry(entity);
            if (entry != null)
                entry.State = EntityState.Detached;
        }

        public virtual void Attach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            if (dbContext.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
        }

        #endregion

        #region Explicit Loading

        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = dbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadCollection<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);
            var collection = dbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                collection.Load();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);
            var reference = dbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadReference<TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);
            var reference = dbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();
        }

        #endregion
    }
}