﻿
namespace ThinkNet.Messaging
{
    using ThinkNet.Infrastructure;

    public interface IEventPublishedVersionStore
    {
        /// <summary>
        /// 更新版本号
        /// </summary>
        void AddOrUpdatePublishedVersion(SourceKey sourceInfo, int version);

        /// <summary>
        /// 获取已发布的版本号
        /// </summary>
        int GetPublishedVersion(SourceKey sourceInfo);
    }
}
