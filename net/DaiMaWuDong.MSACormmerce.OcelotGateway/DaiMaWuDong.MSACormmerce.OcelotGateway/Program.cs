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

            //多添加一个Json数据源
            builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

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
            //支持Token
            builder.Services.AddSwaggerGen(options =>
            {
                #region Swagger配置支持Token参数传递 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer jwtToken(注意中间必须有空格)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//添加安全定义
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {   //添加安全要求
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
            builder.Services.AddOcelot()//Ocelot如何处理
              .AddConsul()//支持Consul
              .AddCacheManager(x =>
              {
                  x.WithDictionaryHandle();//默认字典存储
              })
              .AddPolly()
              ;
            #endregion

            #region jwt校验  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            string authenticationProviderKey = "UserGatewayKey";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Bearer Scheme
                  .AddJwtBearer(authenticationProviderKey, options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                          ValidateIssuer = true,//是否验证Issuer
                          ValidateAudience = true,//是否验证Audience
                          ValidateLifetime = true,//是否验证失效时间---默认还添加了300s后才过期
                          ClockSkew = TimeSpan.FromSeconds(0),//token过期后立马过期
                          ValidateIssuerSigningKey = true,//是否验证SecurityKey
                          ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设一致
                          ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到 SecurityKey
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

            #region Options预请求处理
            app.UsePreOptionsRequest();
            #endregion

            #region 常规ocelot
            app.UseOcelot();//直接替换了管道模型
            #endregion

            //app.UseAuthorization();
            //app.MapControllers();

            app.Run();
        }
    }
}