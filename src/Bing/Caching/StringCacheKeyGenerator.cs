﻿using Bing.Utils.Extensions;

namespace Bing.Caching
{
    /// <summary>
    /// 字符串缓存键生成器
    /// </summary>
    public class StringCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public string GetKey(params object[] args)
        {
            args.CheckNotNullOrEmpty(nameof(args));
            return args.ExpandAndToString(":");
        }
    }
}
