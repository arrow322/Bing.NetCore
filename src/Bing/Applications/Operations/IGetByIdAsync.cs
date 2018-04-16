﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Bing.Applications.Dtos;

namespace Bing.Applications.Operations
{
    /// <summary>
    /// 获取指定标识实体
    /// </summary>
    /// <typeparam name="TDto">数据传输对象类型</typeparam>
    public interface IGetByIdAsync<TDto> where TDto : IDto, new()
    {
        /// <summary>
        /// 通过编号获取
        /// </summary>
        /// <param name="id">实体编号</param>
        /// <returns></returns>
        Task<TDto> GetByIdAsync(object id);

        /// <summary>
        /// 通过编号列表获取
        /// </summary>
        /// <param name="ids">用逗号分隔带额Id列表，范例："1,2"</param>
        /// <returns></returns>
        Task<List<TDto>> GetByIdsAsync(string ids);
    }
}
