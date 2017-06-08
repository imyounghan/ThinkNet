﻿
namespace ThinkNet.Messaging.Handling
{
    /// <summary>
    /// 表示继承该接口的是一个溯源事件的上下文
    /// </summary>
    public interface IEventContext
    {
        /// <summary>
        /// 源信息
        /// </summary>
        SourceInfo SourceInfo { get; }

        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 添加一个命令到当前上下文
        /// </summary>
        void AddCommand(Command command);
    }
}
