﻿

namespace ThinkNet.Messaging
{
    using System.Collections.Generic;

    /// <summary>
    /// 表示一个消息总线的接口
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    public interface IMessageBus<TMessage> where TMessage : IMessage
    {
        void Send(TMessage message);

        void Send(Envelope<TMessage> message);

        void Send(IEnumerable<TMessage> messages);

        void Send(IEnumerable<Envelope<TMessage>> messages);
    }
}
