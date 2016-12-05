﻿using System;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;


namespace ThinkNet.Database
{
    public sealed class NHibernateSessionBuilder
    {
        private static object sync = new object();
        private static NHibernateSessionBuilder instance;
        /// <summary>
        /// 一个工厂实例
        /// </summary>
        public static NHibernateSessionBuilder Instance
        {
            get
            {
                if (instance == null) {
                    lock (sync) {
                        if (instance == null) {
                            instance = new NHibernateSessionBuilder();
                        }
                    }
                }

                return instance;
            }
        }


        /// <summary>
        /// The session factory instance.
        /// </summary>
        private ISessionFactory sessionFactory;
        public ISessionFactory SessionFactory
        {
            get
            {
                return sessionFactory;
            }
        }

        /// <summary>
        /// 使用单独的配置文件构造实例
        /// </summary>
        private NHibernateSessionBuilder()
            : this(new Configuration())
        { }

        /// <summary>
        /// 根据配置文件构造NHibernateSessionFactory
        /// </summary>
        internal NHibernateSessionBuilder(Configuration nhibernateConfig)
        {
            sessionFactory = nhibernateConfig.BuildSessionFactory();
        }

        /// <summary>
        /// 获取当前上下文的session
        /// </summary>
        /// <returns></returns>
        public ISession GetSession()
        {
            return sessionFactory.GetCurrentSession();
        }

        /// <summary>
        /// 打开一个新的session
        /// </summary>
        /// <returns></returns>
        public ISession OpenSession()
        {
            return sessionFactory.OpenSession();
        }

        public ISession OpenSession(IDbConnection connection)
        {
            return sessionFactory.OpenSession(connection);
        }


        public static void BuildSessionFactory(Configuration nhibernateConfig)
        {
            if (instance == null) {
                try {
                    instance = new NHibernateSessionBuilder(nhibernateConfig);
                }
                catch(System.Exception ex) {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 使用web.config配置文件创建一个SessionFactory
        /// 因为在web.config里设置mapping属性无效，所以要加入程序集名称
        /// </summary>
        public static void BuildSessionFactory(string assemblyName)
        {
            BuildSessionFactory(new Configuration().AddAssembly(assemblyName));
        }


        public static void BuildSessionFactory(Assembly assembly)
        {
            try {
                BuildSessionFactory(new Configuration().AddAssembly(assembly));
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        //public static void BuildSessionFactory()
        //{
        //    BuildSessionFactory(new Configuration());
        //}
    }
}
