﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bing.Datas.EntityFramework.Extensions;
using Bing.Datas.Stores;
using Bing.Datas.UnitOfWorks;
using Bing.Domains.Entities;
using Bing.Domains.Repositories;
using Bing.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Bing.Datas.Sql;
using Bing.Helpers;

namespace Bing.Datas.EntityFramework.Core
{
    /// <summary>
    /// 查询存储器
    /// </summary>
    /// <typeparam name="TEntity">对象类型</typeparam>
    public abstract class QueryStoreBase<TEntity> : QueryStoreBase<TEntity, Guid>, IQueryStore<TEntity>
        where TEntity : class, IKey<Guid>
    {
        /// <summary>
        /// 初始化一个<see cref="QueryStoreBase{TEntity}"/>类型的实例
        /// </summary>
        /// <param name="unitOfWork">工作单元</param>
        protected QueryStoreBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    /// <summary>
    /// 查询存储器
    /// </summary>
    /// <typeparam name="TEntity">对象类型</typeparam>
    /// <typeparam name="TKey">对象标识类型</typeparam>
    public abstract class QueryStoreBase<TEntity,TKey>:IQueryStore<TEntity,TKey>where TEntity:class,IKey<TKey>
    {        
        #region 属性

        /// <summary>
        /// 工作单元
        /// </summary>
        protected UnitOfWorkBase UnitOfWork { get; }

        /// <summary>
        /// 实体集
        /// </summary>
        protected DbSet<TEntity> Set => UnitOfWork.Set<TEntity>();

        /// <summary>
        /// Sql查询对象
        /// </summary>
        private ISqlQuery _sqlQuery;

        /// <summary>
        /// Sql查询对象
        /// </summary>
        protected virtual ISqlQuery Sql => _sqlQuery ?? (_sqlQuery = CreateSqlQuery());

        /// <summary>
        /// 创建Sql查询对象
        /// </summary>
        /// <returns></returns>
        protected virtual ISqlQuery CreateSqlQuery()
        {
            var result = Ioc.Create<ISqlQuery>();
            result.SetConnection(Connection);
            return result;
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected IDbConnection Connection => UnitOfWork.Database.GetDbConnection();

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化一个<see cref="QueryStoreBase{TEntity,TKey}"/>类型的实例
        /// </summary>
        /// <param name="unitOfWork">工作单元</param>
        protected QueryStoreBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = (UnitOfWorkBase) unitOfWork;
        }

        #endregion

        /// <summary>
        /// 获取未跟踪查询对象
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> FindAsNoTracking()
        {
            return Set.AsNoTracking();
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Find()
        {
            return Set;
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="criteria">查询条件</param>
        /// <returns></returns>
        public IQueryable<TEntity> Find(ICriteria<TEntity> criteria)
        {
            return Set.Where(criteria);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns></returns>
        public virtual TEntity Find(object id)
        {
            if (id.SafeString().IsEmpty())
            {
                return null;
            }

            return Set.Find(id);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id.SafeString().IsEmpty())
            {
                return null;
            }

            return await Set.FindAsync(new[] {id}, cancellationToken);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIds(params TKey[] ids)
        {
            return FindByIds((IEnumerable<TKey>) ids);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIds(IEnumerable<TKey> ids)
        {
            if(ids==null)
            {
                return null;
            }

            return Find(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">逗号分隔的标识列表，范例："1,2"</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIds(string ids)
        {
            var idList = Conv.ToList<TKey>(ids);
            return FindByIds(idList);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindByIdsAsync(params TKey[] ids)
        {
            return await FindByIdsAsync((IEnumerable<TKey>) ids);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        public virtual async Task<List<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ids == null)
                return null;
            return await Find(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="ids">逗号分隔的标识列表，范例："1,2"</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindByIdsAsync(string ids)
        {
            var idList = Conv.ToList<TKey>(ids);
            return await FindByIdsAsync(idList);
        }

        /// <summary>
        /// 查找未跟踪单个实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns></returns>
        public virtual TEntity FindByIdNoTracking(TKey id)
        {
            var entities = FindByIdsNoTracking(id);
            if (entities == null || entities.Count == 0)
            {
                return null;
            }

            return entities[0];
        }

        /// <summary>
        /// 查找未跟踪单个实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindByIdNoTrackingAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = await FindByIdsNoTrackingAsync(id);
            if (entities == null || entities.Count == 0)
            {
                return null;
            }

            return entities[0];
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIdsNoTracking(params TKey[] ids)
        {
            return FindByIdsNoTracking((IEnumerable<TKey>) ids);
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIdsNoTracking(IEnumerable<TKey> ids)
        {
            if (ids == null)
            {
                return null;
            }

            return FindAsNoTracking().Where(t => ids.Contains(t.Id)).ToList();
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">逗号分隔的标识列表，范例："1,2"</param>
        /// <returns></returns>
        public virtual List<TEntity> FindByIdsNoTracking(string ids)
        {
            var idList = Conv.ToList<TKey>(ids);
            return FindByIdsNoTracking(idList);
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindByIdsNoTrackingAsync(params TKey[] ids)
        {
            return await FindByIdsNoTrackingAsync((IEnumerable<TKey>) ids);
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindByIdsNoTrackingAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ids == null)
            {
                return null;
            }

            return await FindAsNoTracking().Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="ids">逗号分隔的标识列表，范例："1,2"</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindByIdsNoTrackingAsync(string ids)
        {
            var idList = Conv.ToList<TKey>(ids);
            return await FindByIdsNoTrackingAsync(idList);
        }

        /// <summary>
        /// 查找单个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 查找单个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Set.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual List<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return Set.ToList();
            }

            return Find(predicate).ToList();
        }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await Set.ToListAsync();
            }

            return await Find(predicate).ToListAsync();
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual List<TEntity> FindAllNoTracking(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return FindAsNoTracking().ToList();
            }

            return FindAsNoTracking().Where(predicate).ToList();
        }

        /// <summary>
        /// 查找实体列表，不跟踪
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindAllNoTrackingAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await FindAsNoTracking().ToListAsync();
            }

            return await FindAsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual bool Exists(params TKey[] ids)
        {
            if (ids == null)
            {
                return false;
            }

            return Exists(t => ids.Contains(t.Id));
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="ids">标识列表</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(params TKey[] ids)
        {
            if (ids == null)
            {
                return false;
            }

            return await ExistsAsync(t => ids.Contains(t.Id));
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                return false;
            }

            return Set.Any(predicate);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                return false;
            }

            return await Set.AnyAsync(predicate);
        }

        /// <summary>
        /// 查找数量
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return Queryable.Count(Set);
            }

            return Set.Count(predicate);
        }

        /// <summary>
        /// 查找数量
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await Set.CountAsync();
            }

            return await Set.CountAsync(predicate);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual List<TEntity> Query(IQueryBase<TEntity> query)
        {
            return Query(Set, query).ToList();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="queryable">数据源</param>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        private IQueryable<TEntity> Query(IQueryable<TEntity> queryable, IQueryBase<TEntity> query)
        {
            queryable = queryable.Where(query);
            var order = query.GetOrder();
            if (string.IsNullOrWhiteSpace(order))
            {
                return queryable;
            }

            return DynamicQueryableExtensions.OrderBy(queryable, order);
        }

        /// <summary>
        /// 查询 - 返回未跟踪的实体
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual List<TEntity> QueryAsNoTracking(IQueryBase<TEntity> query)
        {
            return Query(FindAsNoTracking(), query).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual PagerList<TEntity> PagerQuery(IQueryBase<TEntity> query)
        {
            return Set.Where(query).ToPagerList(query.GetPager());
        }

        /// <summary>
        /// 分页查询 - 返回未跟踪的实体
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual PagerList<TEntity> PagerQueryAsNoTracking(IQueryBase<TEntity> query)
        {
            return FindAsNoTracking().Where(query).ToPagerList(query.GetPager());
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> QueryAsync(IQueryBase<TEntity> query)
        {
            return await Query(Set, query).ToListAsync();
        }

        /// <summary>
        /// 查询 - 返回未跟踪的实体
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> QueryAsNoTrackingAsync(IQueryBase<TEntity> query)
        {
            return await Query(FindAsNoTracking(), query).ToListAsync();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual async Task<PagerList<TEntity>> PagerQueryAsync(IQueryBase<TEntity> query)
        {
            return await Set.Where(query).ToPagerListAsync(query.GetPager());
        }

        /// <summary>
        /// 分页查询 - 返回未跟踪的实体
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public virtual async Task<PagerList<TEntity>> PagerQueryAsNoTrackingAsync(IQueryBase<TEntity> query)
        {
            return await FindAsNoTracking().Where(query).ToPagerListAsync(query.GetPager());
        }

        
    }
}
