﻿
namespace ThinkNet.Infrastructure
{
    /// <summary>
    /// 表示继承该接口的是一个任务
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
