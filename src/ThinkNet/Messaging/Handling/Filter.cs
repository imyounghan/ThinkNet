﻿

namespace ThinkNet.Messaging.Handling
{
    /// <summary>
    /// 表示一个元数据类，它包含对一个或多个过滤器接口的实现、过滤器顺序和过滤器范围的引用。
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// 表示一个用于指定过滤器的默认顺序的常数。
        /// </summary>
        public const int DefaultOrder = -1;

        /// <summary>
        /// 初始化 <see cref="Filter"/> 类的新实例。
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="scope">范围</param>
        /// <param name="order">顺序</param>
        public Filter(object instance, FilterScope scope, int? order)
        {
            instance.NotNull("instance");

            if(order == null) {
                IFilter filter = instance as IFilter;
                if(filter != null) {
                    order = filter.Order;
                }
            }

            Instance = instance;
            Order = order ?? DefaultOrder;
            Scope = scope;
        }

        /// <summary>
        /// 获取此类的实例。
        /// </summary>
        public object Instance
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取过滤器的应用顺序。
        /// </summary>
        public int Order
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取过滤器的范围排序。
        /// </summary>
        public FilterScope Scope
        {
            get;
            protected set;
        }
    }
}
