using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElectionStatistics.Model
{
    public abstract class Repository<TEntity> : IQueryable<TEntity> where TEntity : class 
    {
        protected Repository(ModelContext context)
        {
            Context = context;
            Expression = Items.Expression;
            ElementType = Items.ElementType;
            Provider = Items.Provider;
        }

        private IQueryable<TEntity> Items
        {
            get { return Context.Set<TEntity>(); }
        }

        protected ModelContext Context { get; private set; }

        public virtual void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
    }
}