using Microsoft.AspNetCore.Hosting;

namespace Seckill.ScheduledProccessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //DateHelper.GetDates(12).ForEach(d => Console.WriteLine(d.ToString("yyyyMMdd HH:mm")));
            //DateHelper.GetDateMenus().ForEach(x => Console.WriteLine(x.ToString("yyyyMMdd HH:mm")));
            /*Parallel.For(0, 10000, (i) =>
            {
                using (RedisClient client = new RedisClient("192.168.3.65"))
                {
                    client.Set("zxname" + i, i);
                    client.Incr("zxname" + i);
                    Console.WriteLine(i);
                }

            });*/
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}