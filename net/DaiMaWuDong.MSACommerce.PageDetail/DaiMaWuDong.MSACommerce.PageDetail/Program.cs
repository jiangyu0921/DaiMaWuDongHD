using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.MSACommerce.Detail.Interface;
using DaiMaWuDong.MSACommerce.Detail.Service;
using log4net.Kafka.Core;

namespace DaiMaWuDong.MSACommerce.PageDetail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region ConfigureBuilder---基础配置
            //builder.Configuration.AddJsonFile
            builder.Host
            //.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            //{
            //    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
            //    configurationBuilder
            //        .AddApollo(configurationBuilder.Build().GetSection("apollo"))
            //        .AddDefault()
            //        .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//自定义的private NameSpace
            //        .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo中默认NameSpace的名称
            //})
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//
                loggingBuilder.AddLog4Net();
            });
            #endregion

            #region ServiceRegister
            // Add services to the container.
            builder.Services.AddControllersWithViews(option =>
            {
                option.Filters.Add<CustomExceptionFilterAttribute>();
                option.Filters.Add(typeof(LogActionFilterAttribute));
            });

            #region 服务注入
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IBrandService, BrandService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IGoodsService, GoodsService>();
            builder.Services.AddTransient<ISpecService, SpecService>();
            builder.Services.AddTransient<IPageDetailService, PageDetailService>();
            builder.Services.AddTransient<RedisClusterHelper>();
            builder.Services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
            #endregion

            #region 配置文件注入
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisClusterOptions>(builder.Configuration.GetSection("RedisConn"));
            #endregion

            #region Consul Server IOC注册
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion

            #endregion
            #region Pipeline
            var app = builder.Build();

            #region 静态化
            if ("true".Equals(builder.Configuration["IsSaveHtml"], StringComparison.OrdinalIgnoreCase))
            {
                app.UseStaticPageMiddleware(builder.Configuration["StaticDirectory"], true, true);
            }
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Consul注册
            app.UseHealthCheckMiddleware("/Health");//心跳请求响应
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            #endregion

            app.Run();
        }
    }
}