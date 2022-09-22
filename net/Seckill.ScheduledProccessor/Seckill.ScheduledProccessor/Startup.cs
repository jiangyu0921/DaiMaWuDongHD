using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Seckill.ScheduledProccessor.Extensions;
using Seckill.ScheduledProccessor.Factory;
using Seckill.ScheduledProccessor.Job;
using Seckill.Common.Helper;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.ScheduledProccessor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<CSRedisHelper>();
            services.AddTransient<YitaoSeckillDBContext>();
            #region Quartz Service
            // Add Quartz services
            services.AddSingleton<IJobFactory, QuartzFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            // Add our job
            services.AddTransient<SeckillGoodsJob>();
            services.AddJob();
            services.AddHostedService<QuartzHostedService>();
            #endregion

            #region 配置文件注入
            // services.Configure<RedisConnOptions>(this.Configuration.GetSection("RedisOptions"));
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
