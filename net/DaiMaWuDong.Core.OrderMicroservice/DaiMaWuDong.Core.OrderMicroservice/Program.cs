using DaiMaWuDong.AgileFramework.AuthenticationCenter;
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.RabbitMQ;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model.Models;
using DaiMaWuDong.MSACommerce.Service;
using log4net.Kafka.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceStack;
using System.Text;

namespace DaiMaWuDong.Core.OrderMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region ConfigureBuilder--��������
            builder.Host
                //.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                //{
                //    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                //    configurationBuilder
                //        .AddApollo(configurationBuilder.Build().GetSection("apollo"))
                //        .AddDefault()
                //        .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//�Զ����private NameSpace
                //        .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo��Ĭ��NameSpace������
                //})
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.Warning);
                    loggingBuilder.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
                    loggingBuilder.AddLog4Net();
                });
            #endregion

            #region ServiceRegister

            builder.Services.AddControllers(option =>
            {
                option.Filters.Add<CustomExceptionFilterAttribute>();
                option.Filters.Add(typeof(LogActionFilterAttribute));
            })
               .AddNewtonsoftJson(options =>
               {
                   //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                   //options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
               });//ASP.NET CoreĬ��Json���л�����ֻ�ܷ��ؼ��㣬�����������������

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                #region Swagger����֧��Token�������� 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������token,��ʽΪ Bearer jwtToken(ע���м�����пո�)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//��Ӱ�ȫ����

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {   //��Ӱ�ȫҪ��
                    new OpenApiSecurityScheme
                    {
                        Reference =new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id ="Bearer"
                        }
                    },
                    new string[]{ }
                }
                });
                #endregion
            });
            #endregion

            #region ����ע��

            builder.Services.AddHttpClient();
            builder.Services.AddTransient<OrangeContext>();
            builder.Services.AddTransient<IGoodsService, GoodsService>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IStockService, StockService>();
            builder.Services.AddTransient<RedisClusterHelper>();
            builder.Services.AddSingleton<RabbitMQInvoker>();
            builder.Services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
            #endregion

            #region Consul Server IOCע��
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion

            #region �����ļ�ע��
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisClusterOptions>(builder.Configuration.GetSection("RedisConn"));
            builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQOptions"));
            #endregion


            #region ���CAP֧��
            builder.Services.AddCap(x =>
            {
                // ������ SqlServer ʹ�õ� EF �������ݲ���������Ҫ����������ã�
                // ע��: �㲻��Ҫ�ٴ����� x.UseSqlServer(""")
                x.UseEntityFramework<OrangeContext>();
                //����RabbitMq��Ϣ
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
            });
            #endregion

            #region jwtУ��  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = false,//�Ƿ���֤ʧЧʱ��
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });
            #endregion

            #region ��ȨҪ��
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ComplexPolicy",
                    policyBuilder => policyBuilder
                    .RequireRole("Admin")//Claim��Role��Admin
                    .RequireUserName("Eleven")//Claim��Name��Eleven
                                              //.RequireClaim(ClaimTypes.Email)//������ĳ��Cliam--����
                    .RequireClaim("EMail")//������ĳ��Cliam-����
                    .RequireClaim("Account")
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Role")
                        && context.User.Claims.First(c => c.Type.Equals("Role")).Value.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
                    .Requirements.Add(new DoubleEmailRequirement())
                    );//����
            });
            builder.Services.AddSingleton<IAuthorizationHandler, DaiMaWuDongMailHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion


            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Consulע��
            app.UseHealthCheckMiddleware("/Health");//����������Ӧ
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion
            app.UseHttpsRedirection();
            #region ��Ӽ�Ȩ
            app.UseAuthentication();
            #endregion
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}