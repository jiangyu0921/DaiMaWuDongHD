using DaiMaWuDong.AgileFramework.AuthenticationCenter;
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using DaiMaWuDong.MSACommerce.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MySqlConnOptions = DaiMaWuDong.Common.Model.MySqlConnOptions;

namespace DaiMaWuDong.Core.UserApi
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
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                loggingBuilder.AddLog4Net();
            });
            #endregion

            #region ServiceRegister

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            //支持Token
            #region Swagger配置支持Token参数传递 

            builder.Services.AddSwaggerGen(options =>
            {

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

            });

            #endregion

            #region 服务注入
            builder.Services.AddTransient<OrangeContext, OrangeContext>();
            builder.Services.AddTransient<CacheClientDB, CacheClientDB>();
            builder.Services.AddTransient<IUserService, UserService>();
            //builder.Services.AddTransient<ISqlSugarCompanyService, SqlSugarCompanyService>();
            builder.Services.AddTransient<RedisClusterHelper>();
            #endregion

            #region 配置文件注入
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisConnOptions>(builder.Configuration.GetSection("RedisConn"));
            builder.Services.Configure<RedisClusterOptions>(builder.Configuration.GetSection("RedisConn"));
            #endregion

            #region jwt校验  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = false,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });
            #endregion


            #region 授权要求
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ComplexPolicy",
                    policyBuilder => policyBuilder
                    .RequireRole("Admin")//Claim的Role是Admin
                    .RequireUserName("Eleven")//Claim的Name是Eleven
                    //.RequireClaim(ClaimTypes.Email)//必须有某个Cliam--不行
                    .RequireClaim("EMail")//必须有某个Cliam-可以
                    .RequireClaim("Account")
                    .RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Role")
                        && context.User.Claims.First(c => c.Type.Equals("Role")).Value.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
                    .Requirements.Add(new DoubleEmailRequirement())
                    );//内置
            });
            builder.Services.AddSingleton<IAuthorizationHandler, DaiMaWuDongMailHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region Consul Server IOC注册
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion

            #endregion

            #region Middleware

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Consul注册
            app.UseHealthCheckMiddleware("/Health");//心跳请求响应
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.UseHttpsRedirection();

            #region 添加鉴权
            app.UseAuthentication();
            #endregion

            app.UseAuthorization();

            #region 跨域取消，网关提供配置
            //app.UseCors("default");
            #endregion

            app.MapControllers();

            app.Run();

            #endregion
        }
    }
}