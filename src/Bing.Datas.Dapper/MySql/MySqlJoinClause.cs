﻿using Bing.Datas.Sql.Queries;
using Bing.Datas.Sql.Queries.Builders.Abstractions;
using Bing.Datas.Sql.Queries.Builders.Clauses;
using Bing.Datas.Sql.Queries.Builders.Core;

namespace Bing.Datas.Dapper.MySql
{
    /// <summary>
    /// MySql 表连接子句
    /// </summary>
    public class MySqlJoinClause : JoinClause
    {
        /// <summary>
        /// 初始化一个<see cref="MySqlJoinClause"/>类型的实例
        /// </summary>
        /// <param name="sqlBuilder">Sql生成器</param>
        /// <param name="dialect">方言</param>
        /// <param name="resolver">实体解析器</param>
        /// <param name="register">实体别名注册器</param>
        public MySqlJoinClause(ISqlBuilder sqlBuilder, IDialect dialect, IEntityResolver resolver,
            IEntityAliasRegister register) : base(sqlBuilder, dialect, resolver, register)
        {
        }

        /// <summary>
        /// 创建连接项
        /// </summary>
        /// <param name="joinType">连接类型</param>
        /// <param name="table">表名</param>
        /// <param name="schema">架构名</param>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        protected override JoinItem CreateJoinItem(string joinType, string table, string schema, string alias)
        {
            return new JoinItem(joinType, table, schema, alias, false, false);
        }
    }
}