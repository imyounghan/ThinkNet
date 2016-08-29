﻿using System;
using System.Diagnostics;
using System.Threading;
using ThinkNet.Configurations;

namespace ThinkNet.Messaging.Processing
{
    public abstract class MessageExecutor<TMessage> : IMessageExecutor
        where TMessage : class, IMessage
    {
        private readonly int _retryTimes;

        protected MessageExecutor()
            : this(ConfigurationSetting.Current.HandleRetrytimes)
        { }
        protected MessageExecutor(int retryTimes)
        {
            this._retryTimes = retryTimes;
        }

        protected abstract void Execute(TMessage message);

        protected virtual void Notify(TMessage message, Exception exception)
        {
            if(exception == null) {
                if(LogManager.Default.IsDebugEnabled) {
                    LogManager.Default.DebugFormat("Handle {0} success.", message);
                }
            }
            else {
                if(LogManager.Default.IsErrorEnabled) {
                    LogManager.Default.Error(exception, "Exception raised when handling {0}.", message);
                }
            }
        }

        bool IMessageExecutor.Execute(IMessage message, out TimeSpan processTime)
        {
            int count = 0;            
            processTime = TimeSpan.Zero;

            while(count++ < _retryTimes) {
                try {
                    var sw = Stopwatch.StartNew();
                    this.Execute(message as TMessage);
                    sw.Stop();
                    processTime = sw.Elapsed;
                    break;
                }
                catch(ThinkNetException ex) {
                    this.Notify(message as TMessage, ex);
                    return false;
                }
                catch(Exception ex) {
                    if(count == _retryTimes) {
                        this.Notify(message as TMessage, ex);
                        return false;
                    }

                    if(LogManager.Default.IsWarnEnabled) {
                        LogManager.Default.Warn(ex,
                            "An exception happened while processing {0} through handler, Error will be ignored and retry again({1}).",
                             message, count);
                    }
                    Thread.Sleep(ConfigurationSetting.Current.HandleRetryInterval);
                }                
            }

            this.Notify(message as TMessage, null);
            return true;
        }
    }
}
