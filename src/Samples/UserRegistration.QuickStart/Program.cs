﻿using System;
using System.Threading;
using ThinkNet;
using UserRegistration.Commands;
using UserRegistration.ReadModel;

namespace UserRegistration.QuickStart
{
    using System.Collections.Generic;
    using System.Linq;

    using ThinkNet.Infrastructure;
    using ThinkNet.Messaging;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Current.Done();


            //var serializer = ObjectContainer.Instance.Resolve<ITextSerializer>();
            //var serialized = serializer.Serialize(new RegisterUser {
            //    UserName = "hanyang",
            //    Password = "123456",
            //    LoginId = "young.han",
            //    Email = "19126332@qq.com"
            //});
            //Console.WriteLine(serialized);


            Console.WriteLine("输入任意键演示...");
            Console.ReadKey();


            Console.WriteLine("开始创建用户...");
            var commandService = ObjectContainer.Instance.Resolve<ICommandService>();
            var commandResult = commandService.Execute(new RegisterUser {
                UserName = "hanyang",
                Password = "123456",
                LoginId = "young.han",
                Email = "19126332@qq.com"
            });
            Console.WriteLine("命令处理完成(结果：{0})...", commandResult.Status);
            //var commandService = ServiceGateway.Current.GetService<ICommandService>();
            //commandService.Execute(new RegisterUser {
            //    UserName = "hanyang",
            //    Password = "123456",
            //    LoginId = "young.han",
            //    Email = "19126332@qq.com"
            //});
            //int counter = 0;
            //var tasks = new System.Threading.Tasks.Task[5000];
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //while(counter < 5000) {
            //    var userRegister = new RegisterUser {
            //        UserName = "hanyang",
            //        Password = "123456",
            //        LoginId = "young.han," + counter.ToString(),
            //        Email = "19126332@qq.com"
            //    };

            //    tasks[counter++] = commandService.ExecuteAsync(userRegister, CommandReturnType.DomainEventHandled);
            //}
            //System.Threading.Tasks.Task.WaitAll(tasks);
            //sw.Stop();
            //Console.WriteLine("用时:{0}ms", sw.ElapsedMilliseconds);
            //Console.WriteLine("成功完成的命令数量：{0}", tasks.Where(p => p.IsCompleted).Count());
            Thread.Sleep(2000);

            var queryService = ObjectContainer.Instance.Resolve<IQueryService>();

            var queryResult = queryService.Execute<IEnumerable<UserModel>>(new FindAllUser());
            if (queryResult.Status != ExecutionStatus.Success)
            {
                Console.WriteLine("查询处理完成(结果：{0})...", queryResult.Status);
            }
            else
            {
                Console.WriteLine("共有 {0} 个用户。", queryResult.Data.Count());
            }
            Thread.Sleep(2000);

            var authoResult =
                queryService.Execute<bool>(
                    new UserAuthentication() { LoginId = "young.han", Password = "123456", IpAddress = "127.0.0.1" });
            if(authoResult.Data) {
                Console.WriteLine("登录成功。");
            }
            else {
                Console.WriteLine("用户名或密码错误。");
            }

            Console.ReadKey();
        }
    }
}
