﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Bing.Extensions;
using Bing.Helpers;
using Bing.Logging.Core;
using Bing.Text;
using Microsoft.Extensions.Logging;

namespace Bing.Logging
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public class Log : ILog
    {
        #region 字段

        /// <summary>
        /// 当前的日志事件描述符
        /// </summary>
        private LogEventDescriptor CurrentDescriptor { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化一个<see cref="Log"/>类型的实例
        /// </summary>
        /// <param name="logger">日志记录包装器</param>
        /// <param name="logContextAccessor">日志上下文访问器</param>
        public Log(ILoggerWrapper logger, ILogContextAccessor logContextAccessor = null)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LogContext = logContextAccessor?.Context;
            LogProperties = new Dictionary<string, object>();
            LogMessage = new StringBuilder();
            LogMessageArgs = new List<object>();
            CurrentDescriptor = new LogEventDescriptor();
        }

        #endregion

        #region Null(空日志操作实例)

        /// <summary>
        /// 空日志操作实例
        /// </summary>
        public static ILog Null = NullLog.Instance;

        #endregion

        #region 属性

        /// <summary>
        /// 日志记录包装器
        /// </summary>
        protected ILoggerWrapper Logger { get; }

        /// <summary>
        /// 日志上下文
        /// </summary>
        protected LogContext LogContext { get; }

        /// <summary>
        /// 日志级别
        /// </summary>
        protected LogLevel LogLevel { get; set; }

        /// <summary>
        /// 日志事件标识
        /// </summary>
        protected EventId LogEventId { get; set; }

        /// <summary>
        /// 日志异常
        /// </summary>
        protected Exception LogException { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        protected IDictionary<string, object> LogProperties { get; set; }

        /// <summary>
        /// 日志状态
        /// </summary>
        protected object LogState { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        protected StringBuilder LogMessage { get; }

        /// <summary>
        /// 日志消息参数
        /// </summary>
        protected List<object> LogMessageArgs { get; }

        #endregion

        #region EventId(设置日志事件标识)

        /// <inheritdoc />
        public virtual ILog EventId(EventId eventId)
        {
            LogEventId = eventId;
            return this;
        }

        #endregion

        #region Exception(设置异常)

        /// <inheritdoc />
        public virtual ILog Exception(Exception exception)
        {
            LogException = exception;
            return this;
        }

        #endregion

        #region Property(设置自定义扩展属性)

        /// <inheritdoc />
        public virtual ILog Property(string propertyName, string propertyValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return this;
            if (LogProperties.ContainsKey(propertyName))
            {
                LogProperties[propertyName] += propertyValue;
                return this;
            }
            LogProperties.Add(propertyName, propertyValue);
            return this;
        }

        #endregion

        #region ExtraProperty(设置扩展属性)

        /// <summary>
        /// 设置扩展属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        public ILog ExtraProperty(string propertyName, object propertyValue)
        {
            CurrentDescriptor.Context.SetExtraProperty(propertyName, propertyValue);
            return this;
        }

        #endregion

        #region Tags(设置标签)

        /// <inheritdoc />
        public virtual ILog Tags(params string[] tags)
        {
            CurrentDescriptor.Context.SetTags(tags);
            return this;
        }

        #endregion

        #region State(设置日志状态对象)

        /// <inheritdoc />
        public virtual ILog State(object state)
        {
            LogState = state;
            return this;
        }

        #endregion

        #region Message(设置日志消息)

        /// <inheritdoc />
        public virtual ILog Message(string message, params object[] args)
        {
            LogMessage.Append(message);
            LogMessageArgs.AddRange(args);
            return this;
        }

        #endregion

        #region IsEnabled(是否启用)

        /// <inheritdoc />
        public virtual bool IsEnabled(LogLevel logLevel) => Logger.IsEnabled(logLevel);

        #endregion

        #region BeginScope(开始日志范围)

        /// <inheritdoc />
        public virtual IDisposable BeginScope<TState>(TState state) => Logger.BeginScope(state);

        #endregion

        #region LogTrace(写跟踪日志)

        /// <inheritdoc />
        public virtual ILog LogTrace([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogTrace(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Trace;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region LogDebug(写调试日志)

        /// <inheritdoc />
        public virtual ILog LogDebug([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogDebug(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Debug;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region LogInformation(写信息日志)

        /// <inheritdoc />
        public virtual ILog LogInformation([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                CurrentDescriptor.Context.SetCallerInfo(memberName, sourceFilePath, sourceLineNumber);
                var scopeDict = CurrentDescriptor.Context.ExposeScopeState();
                if (scopeDict.Any())
                {
                    using (Logger.BeginScope(scopeDict))
                    {
                        Init();
                        if (LogMessage.Length > 0)
                        {
                            Logger.LogInformation(LogEventId, LogException, GetMessage(), GetMessageArgs());
                            return this;
                        }

                        LogLevel = LogLevel.Information;
                        return WriteLog();
                    }
                }
                Debug.WriteLine($"【调试日志】未进入作用域字典");
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogInformation(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Information;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region LogWarning(写警告日志)

        /// <inheritdoc />
        public virtual ILog LogWarning([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogWarning(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Warning;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region LogError(写错误日志)

        /// <inheritdoc />
        public virtual ILog LogError([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogError(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Error;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region LogCritical(写致命日志)

        /// <inheritdoc />
        public virtual ILog LogCritical([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Init();
                if (LogMessage.Length > 0)
                {
                    Logger.LogCritical(LogEventId, LogException, GetMessage(), GetMessageArgs());
                    return this;
                }

                LogLevel = LogLevel.Critical;
                return WriteLog();
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init()
        {
            AddLogContext();
            ConvertStateToContent();
        }

        /// <summary>
        /// 添加日志上下文
        /// </summary>
        protected virtual void AddLogContext()
        {
            if (LogContext == null)
                return;
            if (!string.IsNullOrWhiteSpace(LogContext.TraceId))
                Property("TraceId", LogContext.TraceId);
            if (LogContext.Stopwatch != null)
                Property("Duration", LogContext.Stopwatch.Elapsed.Description());
        }

        /// <summary>
        /// 将状态对象转换到日志内容字典中
        /// </summary>
        protected virtual void ConvertStateToContent()
        {
            if (LogState == null)
                return;
            var state = Conv.ToDictionary(LogState);
            foreach (var item in state)
            {
                if (item.Value.SafeString().IsEmpty())
                    continue;
                LogProperties.Add(item);
            }
        }

        /// <summary>
        /// 获取日志消息
        /// </summary>
        protected virtual string GetMessage()
        {
            if (LogProperties.Count == 0)
                return LogMessage.ToString();
            var result = new StringBuilder();
            result.Append("[");
            foreach (var item in LogProperties)
            {
                result.Append(item.Key);
                result.Append(":{");
                result.Append(item.Key);
                result.Append("},");
            }

            result.TrimEnd(',');
            result.Append("]");
            result.Append(LogMessage);
            return result.ToString();
        }

        /// <summary>
        /// 获取日志消息参数
        /// </summary>
        protected virtual object[] GetMessageArgs()
        {
            if (LogProperties.Count == 0)
                return LogMessageArgs.ToArray();
            var result = new List<object>();
            result.AddRange(LogProperties.Values);
            result.AddRange(LogMessageArgs);
            return result.ToArray();
        }

        /// <summary>
        /// 写日志
        /// </summary>
        protected virtual ILog WriteLog()
        {
            Logger.Log(LogLevel, LogEventId, GetContent(), LogException, GetFormatMessage);
            return this;
        }

        /// <summary>
        /// 获取日志内容
        /// </summary>
        protected IDictionary<string, object> GetContent() => LogProperties.Count == 0 ? null : LogProperties;

        /// <summary>
        /// 获取格式化消息
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="exception">异常</param>
        protected virtual string GetFormatMessage(IDictionary<string, object> content, Exception exception)
        {
            if (exception != null)
                return exception.Message;
            if (content == null)
                return null;
            return GetFormatMessage(content);
        }

        /// <summary>
        /// 获取格式化消息
        /// </summary>
        /// <param name="content">日志内容</param>
        protected virtual string GetFormatMessage(IDictionary<string, object> content)
        {
            var result = new StringBuilder();
            foreach (var item in content)
            {
                result.Append(item.Key);
                result.Append(":");
                result.Append(item.Value.SafeString());
                result.Append(",");
            }

            return result.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 清理状态
        /// </summary>
        protected virtual void Clear()
        {
            LogLevel = LogLevel.None;
            LogEventId = 0;
            LogException = null;
            LogState = null;
            LogProperties = new Dictionary<string, object>();
            LogMessage.Clear();
            LogMessageArgs.Clear();
            CurrentDescriptor = new LogEventDescriptor();
        }

        #endregion
    }
}
