﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ThinkNet.Infrastructure;

namespace ThinkNet.Messaging.Queuing
{
    public class DefaultMessageBroker : IMessageBroker
    {
        class MessageQueue
        {
            private readonly ConcurrentQueue<MetaMessage> _queue;
            private int _running = 1;
            //private long            _offset = 0;
            private int _queueId;

            public MessageQueue()
            {
                this._queue = new ConcurrentQueue<MetaMessage>();
            }


            /// <summary>
            /// 将消息进队列。
            /// </summary>
            public void Enqueue(MetaMessage message)
            {
                message.QueueId = _queueId;
                _queue.Enqueue(message);

                if (_queue.IsEmpty) {
                    Interlocked.Increment(ref runtimes);
                }
            }
            /// <summary>
            /// 取出消息。
            /// </summary>
            public MetaMessage Dequeue()
            {
                MetaMessage message = null;
                if (!_queue.IsEmpty &&
                    Interlocked.CompareExchange(ref _running, 0, 1) == 1 &&
                    _queue.TryDequeue(out message)) {
                    Interlocked.Decrement(ref runtimes);
                    return message;
                }
                return null;
            }

            public void Ack()
            {
                if (Interlocked.CompareExchange(ref _running, 1, 0) == 0) {
                    Interlocked.Increment(ref runtimes);
                }
            }

            /// <summary>
            /// 队列里的消息数量。
            /// </summary>
            public int Count { get { return _queue.Count; } }

            public static MessageQueue[] CreateGroup(int count)
            {
                Ensure.Positive(count, "count");

                MessageQueue[] queues = new MessageQueue[count];
                for (int i = 0; i < count; i++) {
                    queues[i] = new MessageQueue();
                }

                return queues;
            }
        }


        private readonly MessageQueue[] queues;
        private readonly int queueCount;

        private static long            runtimes = 0;
        private int index=0;
        private readonly ConcurrentDictionary<int, int> queueMonitor;
        private readonly BlockingCollection<MetaMessage> currentQueue;

        private EventWaitHandle wait;

        public DefaultMessageBroker()
        {
            this.wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public bool TryAdd(MetaMessage message)
        {
            MessageQueue queue;
            if (queues.Length == 1) {
                queue = queues[0];
            }
            else {
                if (string.IsNullOrWhiteSpace(message.RoutingKey)) {
                    queue = queues.OrderBy(p => p.Count).First();
                }
                else {
                    var queueIndex = message.RoutingKey.GetHashCode() % queueCount;
                    if (queueIndex < 0) {
                        queueIndex = Math.Abs(queueIndex);
                    }
                    queue = queues[queueIndex];
                }
            }

            if (queue.Count >= 1000) {
                return false;
            }

            queue.Enqueue(message);
            if (Interlocked.Read(ref runtimes) == 0) {
                wait.Set();
            }
            return true;
        }

        public bool TryTake(out MetaMessage message)
        {
            var queueIndex = Interlocked.Increment(ref index) % queueCount;
            message = queues[queueIndex].Dequeue();

            return message != null;
        }

        public MetaMessage Take()
        {
            if (Interlocked.Read(ref runtimes) == 0) {
                wait.WaitOne();
            }

            MetaMessage message;
            while (!this.TryTake(out message)) {
                return message;
            }

            return null;
        }

        public void Complete(MetaMessage message)
        {
            queues[message.QueueId].Ack();
            if (Interlocked.Read(ref runtimes) == 0) {
                wait.Set();
            }
        }
    }
}
