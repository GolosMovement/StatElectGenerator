using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectionStatistics.Model
{
    public abstract class CachedRepository<TEntity> : Repository<TEntity> where TEntity : class
    {
        private readonly Lazy<IList<TEntity>> cache;

        protected CachedRepository(ModelContext context) : base(context)
        {
            cache = new Lazy<IList<TEntity>>(() => this.ToList());
        }

        protected IList<TEntity> Cache 
        { 
            get { return cache.Value; }
        }

        public override void Add(TEntity entity)
        {
            base.Add(entity);
            Cache.Add(entity);
        }
    }
}