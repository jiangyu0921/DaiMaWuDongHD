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

                     #region ����ע��
                     services.AddHttpClient();
                     services.AddTransient<OrangePayContext>();
                     services.AddTransient<IPayService, PayService>();
                     services.AddTransient<IOrderService, OrderService>();
                     services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
                     services.AddWechatPay();
                     services.AddTransient<Notify, ResultNotify>();
                     #endregion

                     #region �����ļ�ע��
                     services.Configure<MySqlConnOptions>(Configuration.GetSection("MysqlConn"));
                     services.Configure<ConsulClientOptions>(Configuration.GetSection("ConsulClientOption"));
                     #endregion

                     services.AddHostedService<RefreshPayStatusWorker>();

                     #region CAP����
                     services.AddCap(x =>
                     {
                         x.UseEntityFramework<OrangePayContext>();
                         x.UseRabbitMQ(rb =>
                         {
                             //RabbitMq���ڷ�������ַ
                             rb.HostName = "192.168.1.103";
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
                         x.FailedRetryCount = 10;
                         x.FailedRetryInterval = 60;
                         x.FailedThresholdCallback = failed =>
                         {
                             var logger = failed.ServiceProvider.GetService<ILogger<Program>>();
                             logger.LogError($@"MessageType {failed.MessageType} ʧ���ˣ� ������ {x.FailedRetryCount} ��, 
                        ��Ϣ����: {failed.Message.GetName()}");//do anything
                         };
                     });
                     #endregion
                 })
                .Build();

            host.Run();
        }
    }
}