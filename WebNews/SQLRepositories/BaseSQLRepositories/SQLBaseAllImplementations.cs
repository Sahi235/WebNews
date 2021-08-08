using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.BaseInterfaces;

namespace WebNews.SQLRepositories.BaseSQLRepositories
{
    public class SQLBaseAllImplementations<T> : SQLBaseRepository<T>, IListRepository<T>, IIQueryableRepository<T>, ISingleRepository<T> where T : class
    {
        private readonly DatabaseContext context;
        public SQLBaseAllImplementations(DatabaseContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<T>> GetOrderByConditionTakeSelectInList(Expression<Func<T, object>> orderByExpression, Expression<Func<T, bool>> conditionExpression, int take, Expression<Func<T, T>> selectExpression)
        {
            return await context.Set<T>().AsNoTracking().OrderByDescending(orderByExpression).Where(conditionExpression).Take(take).Select(selectExpression).ToListAsync();
        }

        public async Task<T> GetT(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetTByConditionInIQueryable(Expression<Func<T, bool>> expression)
        {
            return context.Set<T>().Where(expression);
        }

        public async Task<List<T>> GetTByConditionInList(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().AsNoTracking().Where(expression).ToListAsync();
        }

        public async Task<List<T>> GetTByConditionTakeInList(Expression<Func<T, bool>> expression, int take)
        {
            return await context.Set<T>().AsNoTracking().Where(expression).Take(take).ToListAsync();
        }

        public IQueryable<T> GetTByOrderByDescInQueryable(Expression<Func<T, object>> expression)
        {
            return context.Set<T>().OrderByDescending(expression);
        }

        public async Task<List<T>> GetTByOrderByDescTakeInList(Expression<Func<T, object>> expression, int take)
        {
            return await context.Set<T>().AsNoTracking().OrderByDescending(expression).Take(take).ToListAsync();
        }

        public async Task<List<T>> GetTByOrderByInList(Expression<Func<T, DateTime>> expression)
        {
            return await context.Set<T>().AsNoTracking().OrderBy(expression).ToListAsync();
        }

        public async Task<List<T>> GetTByOrderByInList(Expression<Func<T,object>> expression)
        {
            return await context.Set<T>().AsNoTracking().OrderBy(expression).ToListAsync();
        }



        public IQueryable<T> GetTByOrderByInQueryable(Expression<Func<T, object>> expression)
        {
            return context.Set<T>().OrderBy(expression);
        }

        public async Task<List<T>> GetTByOrderByTakeInList(Expression<Func<T, object>> expression, int take)
        {
            return await context.Set<T>().AsNoTracking().OrderBy(expression).Take(take).ToListAsync();
        }

        public async Task<List<T>> GetTBySelectInList(Expression<Func<T, T>> expression)
        {
            return await context.Set<T>().AsNoTracking().Select(expression).ToListAsync();
        }

        public async Task<List<T>> GetTBySelectTakeInList(Expression<Func<T, T>> expression, int take)
        {
            return await context.Set<T>().AsNoTracking().Select(expression).Take(take).ToListAsync();
        }

        public async Task<List<T>> GetTBySelectTakeOrderByDescInList(Expression<Func<T, T>> expression, int take, Expression<Func<T, bool>> condition)
        {
            return await context.Set<T>().AsNoTracking().Select(expression).OrderByDescending(condition).Take(take).ToListAsync();
        }

        public async Task<List<T>> GetTBySelectTakeOrderByInList(Expression<Func<T, T>> expression, int take, Expression<Func<T, bool>> condition)
        {
            return await context.Set<T>().AsNoTracking().Select(expression).OrderBy(condition).Take(take).ToListAsync();
        }

        public async Task<T> GetTConditionSingle(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().SingleOrDefaultAsync(expression);
        }

        public async Task<List<T>> GetTTByOrderByDescInList(Expression<Func<T, object>> expression)
        {
            return await context.Set<T>().AsNoTracking().OrderByDescending(expression).ToListAsync();
        }

        public async Task<List<T>> GetWhereOrderByDescTakeSelect(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderByDesc, int take, Expression<Func<T, T>> select)
        {
            return await context.Set<T>().Where(where).OrderByDescending(orderByDesc).Take(take).Select(select).ToListAsync();
        }

        public async Task<List<T>> GetWhereSkipOrderByDescTakeSelectInList(Expression<Func<T, bool>> where, int skip, Expression<Func<T, object>> orderByDesc, int take, Expression<Func<T, T>> select)
        {
            return await context.Set<T>().AsNoTracking().Where(where).Skip(skip).OrderByDescending(orderByDesc).Take(take).Select(select).ToListAsync();
        }
    }
}
