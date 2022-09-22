using Seckill.Common.Helper;
using Seckill.EntityFrameworkCore.Modles;

namespace Seckill.PreheatProccessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<CSRedisHelper>();
                    services.AddTransient<YitaoSeckillDBContext>();

                    services.AddHostedService<WarmupWorker>();
                })
                .Build();

            host.Run();
        }
    }
}