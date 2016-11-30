﻿using System;
using System.Collections;
using System.Runtime.Serialization;
using ThinkNet.Contracts;

namespace ThinkNet.Messaging
{
    /// <summary>
    /// 命令处理结果的回复
    /// </summary>
    [DataContract]
    [Serializable]
    public sealed class CommandResult : ICommandResult, IMessage, IUniquelyIdentifiable
    {
        /// <summary>
        /// 命令处理状态。
        /// </summary>
        [DataMember]
        public CommandStatus Status { get; set; }
        /// <summary>
        /// Represents the unique identifier of the command.
        /// </summary>
        [DataMember]
        public string CommandId { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }
        /// <summary>
        /// 设置或获取一个提供用户定义的其他异常信息的键/值对的集合。
        /// </summary>
        [DataMember]
        public IDictionary ErrorData { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        [DataMember]
        public CommandReturnType CommandReturnType { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [DataMember]
        public DateTime ReplyTime { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CommandResult()
        { }
        
        /// <summary>
        /// 表示成功的结果
        /// </summary>
        public CommandResult(string commandId, CommandReturnType commandReturnType = CommandReturnType.CommandExecuted)
            : this(commandId, commandReturnType, CommandStatus.Success, null)
        { }

        /// <summary>
        /// 表示失败的结果
        /// </summary>
        public CommandResult(string commandId, string errorMessage, string errorCode = "-1", CommandReturnType commandReturnType = CommandReturnType.CommandExecuted)
            : this(commandId, commandReturnType, CommandStatus.Failed, null, errorCode)
        { }

        /// <summary>
        /// Parameterized Constructor.
        /// </summary>
        public CommandResult(string commandId, CommandReturnType commandReturnType, CommandStatus status, string errorMessage = null, string errorCode = null)
        {
            this.CommandId = commandId;
            this.CommandReturnType = commandReturnType;
            this.ReplyTime = DateTime.UtcNow;
            this.Status = status;
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;
        }
        /// <summary>
        /// Parameterized Constructor.
        /// </summary>
        public CommandResult(string commandId, Exception exception, CommandReturnType commandReturnType = CommandReturnType.CommandExecuted)
        {
            this.CommandId = commandId;
            this.CommandReturnType = commandReturnType;
            this.ReplyTime = DateTime.UtcNow;
            this.Status = CommandStatus.Success;

            if(exception == null)
                return;

            this.Status = CommandStatus.Failed;
            this.ErrorMessage = exception.Message;
            this.ErrorData = exception.Data;

            var thinkNetException = exception as ThinkNetException;
            if(thinkNetException != null)
                this.ErrorCode = thinkNetException.MessageCode;
            else
                this.ErrorCode = "-1";
        }



        /// <summary>
        /// 输出该命令结果的字符串形式
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}@{1}#{2},{3}", this.GetType().FullName, this.CommandId, this.CommandReturnType.ToString(), this.Status.ToString());
        }

        string IUniquelyIdentifiable.Id
        {
            get
            {
                return this.CommandId;
            }
        }

        string IMessage.GetKey()
        {
            return this.CommandId;
        }
    }
}
