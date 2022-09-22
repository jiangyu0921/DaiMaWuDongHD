using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.MSACommerce.Detail.Interface;
using DaiMaWuDong.MSACommerce.Detail.Service;

namespace DaiMaWuDong.MSACommerce.StaticPageProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                 {
                     //LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                     //configurationBuilder
                     //    .AddApollo(configurationBuilder.Build().GetSection("apollo"))
                     //    .AddDefault()
                     //    .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//自定义的private     NameSpace
                     //    .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo中默认       NameSpace的名称
                     ;
                 })
                .ConfigureLogging(loggingBuilder =>
                {
                    //loggingBuilder.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Warning);
                    //loggingBuilder.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
                    loggingBuilder.AddLog4Net();
                })
                .ConfigureServices(services =>
                {
                    IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

                    #region 服务注入
                    services.AddHttpClient();
                    services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
                    services.Configure<MySqlConnOptions>(configuration.GetSection("MysqlConn"));
                    services.AddTransient<IGoodsService, GoodsService>();
                    services.AddTransient<CacheClientDB, CacheClientDB>();
                    services.Configure<RedisConnOptions>(configuration.GetSection("RedisConn"));

                    services.AddSingleton<RabbitMQInvoker>();
                    services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQOptions"));
                    #endregion

                    #region Consul
                    services.Configure<ConsulClientOptions>(configuration.GetSection("ConsulClientOption"));
                    services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
                    #endregion

                    //services.AddHostedService<Worker>();
                    services.AddHostedService<InitPageWorker>();
                    services.AddHostedService<WarmupPageWorker>();
                })
                .Build();

            host.Run();
        }
    }
}