using Microsoft.AspNetCore.Mvc;
using Seckill.ScheduledProccessor.Job;

namespace Seckill.ScheduledProccessor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJob(this IServiceCollection services)
        {
            // 必须使用单例注册
            services.AddSingleton(new JobSchedule(
               jobType: typeof(SeckillGoodsJob),
               // 0 0 0/1 * * ?  0/5 * * * * ?
               cronExpression: "0/5 * * * * ?")); // run every 5 seconds
                                                  //cronExpression: "0/10 * * * * ?")); // run every 1 hours
        }
    }
}
