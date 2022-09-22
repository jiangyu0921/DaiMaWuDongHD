using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.MSACommerce.ElasticSearchProcessor;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Service;

namespace DaiMaWuDong.MSACommerce.ElasticSearchProcessor
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
                    #region 配置文件注入
                    IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
                    services.Configure<ElasticSearchOptions>(Configuration.GetSection("ESConn"));
                    services.Configure<MySqlConnOptions>(Configuration.GetSection("MysqlConn"));
                    services.Configure<RedisClusterOptions>(Configuration.GetSection("RedisConn"));
                    #endregion

                    #region 服务注入
                    services.AddSingleton<RabbitMQInvoker>();
                    //services.AddTransient<OrangeContext, OrangeContext>();
                    services.AddTransient<IGoodsService, GoodsService>();
                    services.AddTransient<ISearchService, SearchService>();
                    services.AddTransient<IElasticSearchService, ElasticSearchService>();
                    services.AddTransient<IBrandService, BrandService>();
                    services.AddTransient<ICategoryService, CategoryService>();
                    services.AddTransient<ISpecService, SpecService>();
                    //services.AddTransient<IPageDetailService, PageDetailService>();
                    services.AddTransient<RedisClusterHelper>();
                    #endregion

                    #region Worker
                    services.AddHostedService<InitESIndexWorker>();
                    services.AddHostedService<WarmupESIndexWorker>();
                    // services.AddHostedService<Worker>();
                    #endregion
                })
                .Build();

            host.Run();
        }
    }
}