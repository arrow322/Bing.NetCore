﻿using System;
using System.Collections.Generic;
using Bing.Core.Modularity;
using Bing.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Bing.Core.Builders
{
    /// <summary>
    /// Bing 构建器
    /// </summary>
    public interface IBingBuilder
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 加载的模块集合
        /// </summary>
        IEnumerable<BingModule> Modules { get; }

        /// <summary>
        /// Bing 选项配置委托
        /// </summary>
        Action<BingOptions> OptionsAction { get; }

        /// <summary>
        /// 添加指定模块
        /// </summary>
        /// <typeparam name="TModule">要添加的模块类型</typeparam>
        IBingBuilder AddModule<TModule>() where TModule : BingModule;

        /// <summary>
        /// 添加加载的所有模块，并可排除指定的模块类型
        /// </summary>
        /// <param name="exceptModuleTypes">要排除的模块类型</param>
        IBingBuilder AddModules(params Type[] exceptModuleTypes);

        /// <summary>
        /// 添加Bing选项配置
        /// </summary>
        /// <param name="optionsAction">选项操作</param>
        IBingBuilder AddOptions(Action<BingOptions> optionsAction);
    }
}
