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
                    #region �����ļ�ע��
                    IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
                    services.Configure<MySqlConnOptions>(Configuration.GetSection("MysqlConn"));
                    services.Configure<RedisClusterOptions>(Configuration.GetSection("RedisConn"));
                    #endregion

                    #region ����ע��
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

                    #region ���CAP֧��
                    services.AddCap(x =>
                    {
                        // ������ SqlServer ʹ�õ� EF �������ݲ���������Ҫ����������ã�
                        // ע��: �㲻��Ҫ�ٴ����� x.UseSqlServer(""")
                        x.UseEntityFramework<OrangeContext>();
                        //����RabbitMq��Ϣ
                        x.UseRabbitMQ(rb =>
                        {
                            //RabbitMq���ڷ�������ַ
                            rb.HostName = "192.168.3.254";
                            //���õ��û�����Ĭ�����ɵ���guest�û�������Ҳ��guest��
                            //����������Mq����ӵ�admin�û����������õ�admin��
                            rb.UserName = "guest";
                            //���õ�����
                            rb.Password = "guest";
                            //Ĭ�϶˿�
                            rb.Port = 5672;
                            //һ����������������������ɸ�Exchange��Queue��ͬһ�������������治������ͬ���Ƶ�Exchange��Queue��
                            //��һ����������Ϊ���������·��
                            rb.VirtualHost = "/";
                            //ʹ�õý���������
                            rb.ExchangeName = "CapExchange";
                        });
                    });
                    #endregion

                    #region WorkServiceע��
                    services.AddHostedService<CleanCartWorker>();
                    services.AddHostedService<TimeoutOrderWorker>();
                    #endregion
                })
                .Build();

            host.Run();
        }
    }
}