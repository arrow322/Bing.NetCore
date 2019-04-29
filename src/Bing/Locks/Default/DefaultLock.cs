﻿using System;
using Bing.Caching;

namespace Bing.Locks.Default
{
    /// <summary>
    /// 业务锁
    /// </summary>
    public class DefaultLock : ILock
    {
        /// <summary>
        /// 缓存提供程序
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// 锁定标识
        /// </summary>
        private string _key;

        /// <summary>
        /// 延迟执行时间
        /// </summary>
        private TimeSpan? _expiration;

        /// <summary>
        /// 初始化一个<see cref="DefaultLock"/>类型的实例
        /// </summary>
        /// <param name="cache">缓存提供程序</param>
        public DefaultLock(ICache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 锁定，成功锁定返回true，false代表之前已被锁定
        /// </summary>
        /// <param name="key">锁定标识</param>
        /// <param name="expiration">锁定时间间隔</param>
        /// <returns></returns>
        public bool Lock(string key, TimeSpan? expiration = null)
        {
            _key = key;
            _expiration = expiration;
            if (_cache.Exists(key))
            {
                return false;
            }

            return _cache.TryAdd(key, 1, expiration);
        }

        /// <summary>
        /// 解除锁定
        /// </summary>
        public void UnLock()
        {
            if (_expiration != null)
            {
                return;
            }

            if (_cache.Exists(_key) == false)
            {
                return;
            }

            _cache.Remove(_key);
        }
    }
}
