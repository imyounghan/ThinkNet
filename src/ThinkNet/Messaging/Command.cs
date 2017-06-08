﻿

namespace ThinkNet.Messaging
{
    using System;
    using System.Runtime.Serialization;

    using ThinkNet.Infrastructure;

    /// <summary>
    /// 表示继承该抽象类的是一个命令
    /// </summary>
    [DataContract]
    public abstract class Command : ICommand, IMessage, IKeyProvider
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        protected Command()
        {
            this.Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 生成当前消息的时间戳
        /// </summary>
        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 获取路由的关键字
        /// </summary>
        protected virtual string GetRoutingKey()
        {
            return null;
        }


        #region IKeyProvider 成员

        string IKeyProvider.GetKey()
        {
            return this.GetRoutingKey();
        }

        #endregion
    }
}
