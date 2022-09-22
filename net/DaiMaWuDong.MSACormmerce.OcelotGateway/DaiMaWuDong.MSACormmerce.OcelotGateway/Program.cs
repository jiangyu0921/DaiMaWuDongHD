using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Polly;
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ocelot.Middleware;
using DaiMaWuDong.AgileFramework.MiddlewareExtend;

namespace DaiMaWuDong.MSACormmerce.OcelotGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //�����һ��Json����Դ
            builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

            #region ConfigureBuilder---��������
            //builder.Configuration.AddJsonFile
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
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                loggingBuilder.AddLog4Net();
            });
            #endregion

            #region ServiceRegister
            // Add services to the container.
            //builder.Services.AddControllers();

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            //֧��Token
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

            #region Ocelot
            builder.Services.AddOcelot()//Ocelot��δ���
              .AddConsul()//֧��Consul
              .AddCacheManager(x =>
              {
                  x.WithDictionaryHandle();//Ĭ���ֵ�洢
              })
              .AddPolly()
              ;
            #endregion

            #region jwtУ��  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            string authenticationProviderKey = "UserGatewayKey";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Bearer Scheme
                  .AddJwtBearer(authenticationProviderKey, options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                          ValidateIssuer = true,//�Ƿ���֤Issuer
                          ValidateAudience = true,//�Ƿ���֤Audience
                          ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��---Ĭ�ϻ������300s��Ź���
                          ClockSkew = TimeSpan.FromSeconds(0),//token���ں��������
                          ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                          ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt����һ��
                          ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//�õ� SecurityKey
                      };
                  });
            #endregion

            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot V1");
                        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "AuthenticationCenter  WebAPI V1");
                        c.SwaggerEndpoint("/user/swagger/v1/swagger.json", "UserMicroservice  WebAPI V1");
                        c.SwaggerEndpoint("/search/swagger/v1/swagger.json", "GoodsSearchMicroservice  WebAPI V1");
                        c.SwaggerEndpoint("/cart/swagger/v1/swagger.json", "CartMicroservice  WebAPI V1");
                        c.SwaggerEndpoint("/order/swagger/v1/swagger.json", "OrderMicroservice  WebAPI V1");
                        c.SwaggerEndpoint("/pay/swagger/v1/swagger.json", "PayMicroservice  WebAPI V1");
                    });
            }

            #region OptionsԤ������
            app.UsePreOptionsRequest();
            #endregion

            #region ����ocelot
            app.UseOcelot();//ֱ���滻�˹ܵ�ģ��
            #endregion

            //app.UseAuthorization();
            //app.MapControllers();

            app.Run();
        }
    }
}