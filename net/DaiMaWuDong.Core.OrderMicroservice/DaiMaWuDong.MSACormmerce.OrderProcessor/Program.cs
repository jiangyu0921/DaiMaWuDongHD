using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model.Models;
using DaiMaWuDong.MSACommerce.Service;

namespace DaiMaWuDong.MSACormmerce.OrderProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    #region 配置文件注入
                    IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
                    services.Configure<MySqlConnOptions>(Configuration.GetSection("MysqlConn"));
                    services.Configure<RedisClusterOptions>(Configuration.GetSection("RedisConn"));
                    #endregion

                    #region 服务注入
                    services.AddHttpClient();
                    services.AddTransient<OrangeContext>();
                    services.AddSingleton<RabbitMQInvoker>();
                    services.AddSingleton<RedisClusterHelper>();
                    services.AddTransient<IGoodsService, GoodsService>();
                    services.AddTransient<IOrderService, OrderService>();
                    services.AddTransient<ICartService, CartService>();
                    services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
                    #endregion

                    services.Configure<ConsulClientOptions>(Configuration.GetSection("ConsulClientOption"));

                    #region 添加CAP支持
                    services.AddCap(x =>
                    {
                        // 如果你的 SqlServer 使用的 EF 进行数据操作，你需要添加如下配置：
                        // 注意: 你不需要再次配置 x.UseSqlServer(""")
                        x.UseEntityFramework<OrangeContext>();
                        //配置RabbitMq信息
                        x.UseRabbitMQ(rb =>
                        {
                            //RabbitMq所在服务器地址
                            rb.HostName = "192.168.3.254";
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
                    });
                    #endregion

                    #region WorkService注入
                    services.AddHostedService<CleanCartWorker>();
                    services.AddHostedService<TimeoutOrderWorker>();
                    #endregion
                })
                .Build();

            host.Run();
        }
    }
}