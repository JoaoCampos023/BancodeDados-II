using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Interfaces;
using SistemaTransporteAereo.Infra.Data.Context;

namespace SistemaTransporteAereo.Infra.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TransporteAereoContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(TransporteAereoContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual int Count()
        {
            return DbSet.Count();
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }
    }
}