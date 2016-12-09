﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThinkNet.Runtime.Routing
{
    /// <summary>
    /// <see cref="IEnvelopeSender"/> 和 <see cref="IEnvelopeReceiver"/> 的实现类
    /// </summary>
    public class EnvelopeHub : DisposableObject, IEnvelopeSender, IEnvelopeReceiver
    {
        private readonly BlockingCollection<Envelope>[] brokers;
        private readonly IRoutingKeyProvider _routingKeyProvider;

        private CancellationTokenSource cancellationSource;

        /// <summary>
        /// Parameterized Constructor.
        /// </summary>        
        public EnvelopeHub(IRoutingKeyProvider routingKeyProvider)
            : this(routingKeyProvider, ConfigurationSetting.Current.QueueCount)
        { }
        /// <summary>
        /// Parameterized Constructor.
        /// </summary>
        protected EnvelopeHub(IRoutingKeyProvider routingKeyProvider, int queueCount)
        {
            this._routingKeyProvider = routingKeyProvider;
            this.brokers = new BlockingCollection<Envelope>[queueCount];

            for(int i = 0; i < queueCount; i++) {
                this.brokers[i] = new BlockingCollection<Envelope>();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void Dispose(bool disposing)
        { }

        private BlockingCollection<Envelope> GetBroker(string routingKey)
        {
            var processorCount = this.brokers.Length;

            if(processorCount == 1) {
                return this.brokers[0];
            }

            if(string.IsNullOrWhiteSpace(routingKey)) {
                return this.brokers.OrderBy(broker => broker.Count).First();
            }

            var index = Math.Abs(routingKey.GetHashCode() % processorCount);
            return this.brokers[index];
        }

        /// <summary>
        /// 获取关键字
        /// </summary>
        protected virtual string GetKey(Envelope envelope)
        {
            return envelope.GetMetadata(StandardMetadata.SourceId)
                .IfEmpty(() => _routingKeyProvider.GetRoutingKey(envelope.Body));
        }

        /// <summary>
        /// 路由信件
        /// </summary>
        protected void Route(Envelope envelope)
        {
            this.GetBroker(this.GetKey(envelope)).Add(envelope, this.cancellationSource.Token);
        }

        /// <summary>
        /// 收到信件后的处理方式
        /// </summary>
        public event EventHandler<Envelope> EnvelopeReceived = (sender, args) => { };

        private void ReceiveMessages(object state)
        {
            var broker = state as BlockingCollection<Envelope>;
            broker.NotNull("state");

            //while(!cancellationSource.IsCancellationRequested) {
            //    var item = broker.Take(cancellationSource.Token);
            //    if(LogManager.Default.IsDebugEnabled) {
            //        LogManager.Default.DebugFormat("Receive an envelope from local queue, data:({0}).", item.Body);
            //    }
            //    this.EnvelopeReceived(this, item);
            //}
            foreach(var item in broker.GetConsumingEnumerable(cancellationSource.Token)) {
                if(LogManager.Default.IsDebugEnabled) {
                    LogManager.Default.DebugFormat("Receive an envelope from local queue, data:({0}).", item.Body);
                }
                this.EnvelopeReceived(this, item);
            }
        }

        void IEnvelopeReceiver.Start()
        {
            if(this.cancellationSource == null) {
                this.cancellationSource = new CancellationTokenSource();

                foreach(var broker in this.brokers) {
                    Task.Factory.StartNew(this.ReceiveMessages,
                        broker,
                        this.cancellationSource.Token,
                        TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness,
                        TaskScheduler.Current);
                }
            }
        }

        void IEnvelopeReceiver.Stop()
        {
            if (this.cancellationSource != null) {
                using (this.cancellationSource) {
                    this.cancellationSource.Cancel();
                    this.cancellationSource = null;
                }
            }
        }

        /// <summary>
        /// Sends an envelope.
        /// </summary>
        public virtual Task SendAsync(Envelope envelope)
        {
            if(LogManager.Default.IsDebugEnabled) {
                LogManager.Default.DebugFormat("Send an envelope to local queue, data({0}).", envelope.Body);
            }

            return Task.Factory.StartNew(() => this.Route(envelope));
        }
        /// <summary>
        /// Sends a batch of envelopes.
        /// </summary>
        public virtual Task SendAsync(IEnumerable<Envelope> envelopes)
        {
            if(LogManager.Default.IsDebugEnabled) {
                LogManager.Default.DebugFormat("Send a batch of envelope to local queue, data:(0).", 
                    string.Join(";", envelopes.Select(item=>item.Body.ToString())));
            }

            return Task.Factory.StartNew(delegate {
                envelopes.ForEach(this.Route);
            });
        }
    }
}
