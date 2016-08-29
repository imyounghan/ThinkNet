﻿using System;
using System.Runtime.Serialization;
using ThinkNet.Infrastructure;

namespace ThinkNet.Messaging
{
    /// <summary>
    /// 实现 <see cref="IMessage"/> 的抽象类
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        protected Message()
        {
            this.CreatedTime = DateTime.UtcNow;
        }
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        protected Message(string id)
            : this()
        {
            this.Id = id.IfEmpty(ObjectId.GenerateNewStringId);
        }

        /// <summary>
        /// 事件标识
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; protected set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember(Name = "createdTime")]
        public DateTime CreatedTime { get; private set; }

        /// <summary>
        /// 输出该类型的字符串
        /// </summary>
        public override string ToString()
        {
            return string.Concat(this.GetType().FullName, "@", this.Id);
        }

    } 
}
