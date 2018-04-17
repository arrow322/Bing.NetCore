﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bing.Datas.Queries.Trees
{
    /// <summary>
    /// 树型查询参数
    /// </summary>
    /// <typeparam name="TParentId">父编号类型</typeparam>
    public interface ITreeQueryParameter<TParentId>:IQueryParameter
    {
        /// <summary>
        /// 父编号
        /// </summary>
        TParentId ParentId { get; set; }

        /// <summary>
        /// 级数
        /// </summary>
        int? Level { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        bool? Enabled { get; set; }

        /// <summary>
        /// 是否搜索
        /// </summary>
        /// <returns></returns>
        bool IsSearch();
    }

    /// <summary>
    /// 树型查询参数
    /// </summary>
    public interface ITreeQueryParameter : ITreeQueryParameter<Guid?>
    {
    }
}
