﻿using System;
using System.Linq.Expressions;
using Bing.Domains.Repositories;
using Bing.Utils;
using Bing.Utils.Expressions;
using Bing.Utils.Helpers;

namespace Bing.Datas.Queries.Criterias
{
    /// <summary>
    /// 范围过滤条件
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public abstract class SegmentCriteriaBase<TEntity, TProperty, TValue> : ICriteria<TEntity> where TEntity : class where TValue : struct
    {
        /// <summary>
        /// 属性表达式
        /// </summary>
        private readonly Expression<Func<TEntity, TProperty>> _propertyExpression;

        /// <summary>
        /// 表达式生成器
        /// </summary>
        private readonly PredicateExpressionBuilder<TEntity> _builder;

        /// <summary>
        /// 最小值
        /// </summary>
        private TValue? _min;

        /// <summary>
        /// 最大值
        /// </summary>
        private TValue? _max;

        /// <summary>
        /// 包含边界
        /// </summary>
        private readonly Boundary _boundary;

        /// <summary>
        /// 初始化一个<see cref="SegmentCriteriaBase{TEntity,TProperty,TValue}"/>类型的实例
        /// </summary>
        /// <param name="propertyExpression">属性表达式</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="boundary">包含边界</param>
        protected SegmentCriteriaBase(Expression<Func<TEntity, TProperty>> propertyExpression, TValue? min, TValue? max,
            Boundary boundary)
        {
            _builder = new PredicateExpressionBuilder<TEntity>();
            _propertyExpression = propertyExpression;
            _min = min;
            _max = max;
            _boundary = boundary;
        }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        /// <returns></returns>
        protected Type GetPropertyType()
        {
            return Lambda.GetType(_propertyExpression);
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> GetPredicate()
        {
            _builder.Clear();
            Adjust(_min, _max);
            CreateLeftExpression();
            CreateRightExpression();
            return _builder.ToLambda();
        }

        /// <summary>
        /// 当最小值大于最大值时进行校正
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private void Adjust(TValue? min, TValue? max)
        {
            if (IsMinGreaterMax(min, max) == false)
            {
                return;
            }
            _min = max;
            _max = min;
        }

        /// <summary>
        /// 最小值是否大于最大值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        protected abstract bool IsMinGreaterMax(TValue? min, TValue? max);

        /// <summary>
        /// 创建左操作数，即 t=> t.Property >= Min
        /// </summary>
        private void CreateLeftExpression()
        {
            if (_min == null)
            {
                return;
            }
            _builder.Append(_propertyExpression, CreateLeftOperator(_boundary), GetMinValueExpression());
        }

        /// <summary>
        /// 创建左操作符
        /// </summary>
        /// <param name="boundary">查询边界</param>
        /// <returns></returns>
        protected virtual Operator CreateLeftOperator(Boundary? boundary)
        {
            switch (boundary)
            {
                case Boundary.Left:
                    return Operator.GreaterEqual;
                case Boundary.Both:
                    return Operator.GreaterEqual;
                default:
                    return Operator.Greater;
            }
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <returns></returns>
        protected TValue? GetMinValue()
        {
            return _min;
        }

        /// <summary>
        /// 获取最小值表达式
        /// </summary>
        /// <returns></returns>
        protected virtual Expression GetMinValueExpression()
        {
            return Lambda.Constant(_min, _propertyExpression);
        }

        /// <summary>
        /// 创建右操作数，即 t => t.Property &lt;= Max
        /// </summary>
        private void CreateRightExpression()
        {
            if (_max == null)
            {
                return;
            }
            _builder.Append(_propertyExpression, CreateRightOperator(_boundary), GetMaxValueExpression());
        }

        /// <summary>
        /// 创建右操作符
        /// </summary>
        /// <param name="boundary">查询边界</param>
        /// <returns></returns>
        protected virtual Operator CreateRightOperator(Boundary? boundary)
        {
            switch (boundary)
            {
                case Boundary.Right:
                    return Operator.LessEqual;
                case Boundary.Both:
                    return Operator.LessEqual;
                default:
                    return Operator.Less;
            }
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <returns></returns>
        protected TValue? GetMaxValue()
        {
            return _max;
        }

        /// <summary>
        /// 获取最大值表达式
        /// </summary>
        /// <returns></returns>
        protected virtual Expression GetMaxValueExpression()
        {
            return Lambda.Constant(_max, _propertyExpression);
        }
    }
}
