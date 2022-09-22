using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.AgileFramework.WechatPayCore;
using DaiMaWuDong.Core.PayMicroservice;
using DaiMaWuDong.Core.PayMicroservice.Domain;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model.Models;
using DaiMaWuDong.MSACommerce.Service;
using DotNetCore.CAP.Messages;

namespace DaiMaWuDong.MSACormmerce.PayProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddSingleton<RabbitMQInvoker>();
                     services.AddSingleton<RedisClusterHelper>();


                     IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

                     services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
                     services.Configure<RedisClusterOptions>(Configuration.GetSection("RedisConn"));

                     #region 服务注入
                     services.AddHttpClient();
                     services.AddTransient<OrangePayContext>();
                     services.AddTransient<IPayService, PayService>();
                     services.AddTransient<IOrderService, OrderService>();
                     services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
                     services.AddWechatPay();
                     services.AddTransient<Notify, ResultNotify>();
                     #endregion

                     #region 配置文件注入
                     services.Configure<MySqlConnOptions>(Configuration.GetSection("MysqlConn"));
                     services.Configure<ConsulClientOptions>(Configuration.GetSection("ConsulClientOption"));
                     #endregion

                     services.AddHostedService<RefreshPayStatusWorker>();

                     #region CAP配置
                     services.AddCap(x =>
                     {
                         x.UseEntityFramework<OrangePayContext>();
                         x.UseRabbitMQ(rb =>
                         {
                             //RabbitMq所在服务器地址
                             rb.HostName = "192.168.1.103";
                             //设置得用户名（默认生成得是guest用户，密码也是guest，
                             //这里是我在Mq里添加得admin用户，密码设置的admin）
                             rb.UserName = "guest";
                             //设置得密码
                             rb.Password = "guest";
                             //默认端口
                             rb.Port = 5672;
                             //一个虚拟主机里面可以有若干个Exchange和Queue，同一个虚拟主机里面不能有相同名称的Exchange或Queue。
                             //将一个主机虚拟为多个，类似路由
                             rb.VirtualHost = "/";
                             //使用得交换机名称
                             rb.ExchangeName = "CapExchange";
                         });
                         x.FailedRetryCount = 10;
                         x.FailedRetryInterval = 60;
                         x.FailedThresholdCallback = failed =>
                         {
                             var logger = failed.ServiceProvider.GetService<ILogger<Program>>();
                             logger.LogError($@"MessageType {failed.MessageType} 失败了， 重试了 {x.FailedRetryCount} 次, 
                        消息名称: {failed.Message.GetName()}");//do anything
                         };
                     });
                     #endregion
                 })
                .Build();

            host.Run();
        }
    }
}