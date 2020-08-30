﻿using System;

namespace Bing.Webs.Filters
{
    /// <summary>
    /// 忽略响应结果处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    [Obsolete("请使用Bing.AspNetCore.Mvc.Filters.IgnoreResultHandlerAttribute")]
    public class IgnoreResultHandlerAttribute : Attribute
    {
    }
}
