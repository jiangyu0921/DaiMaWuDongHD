using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.AuthenticationCenter;
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.HttpApiExtend;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace DaiMaWuDong.MSACommerce.Search.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region 注入HttpInvoker对象
            builder.Services.AddHttpInvoker(options =>
            {
                options.Message = "This is Program's Message";
            });
            #endregion

            #region 使用Autofac
            {
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>((context, buider) =>
                {
                    // 必须使用单例注册
                    buider.RegisterType<BrandService>()
                    .As<IBrandService>().SingleInstance().EnableInterfaceInterceptors();

                    buider.RegisterType<CategoryService>()
                    .As<ICategoryService>().SingleInstance().EnableInterfaceInterceptors();

                    buider.RegisterType<SpecService>()
                    .As<ISpecService>().SingleInstance().EnableInterfaceInterceptors();

                    buider.RegisterType<PollyPolicyAttribute>();

                });
            }
            #endregion

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            #region 支持Token
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

            #region 添加Log4Net支持
            //builder.Logging
            //    .AddFilter("System", LogLevel.Warning)
            //    .AddFilter("Microsoft", LogLevel.Warning)
            //    .AddLog4Net();
            #endregion

            #region 服务注入
            builder.Services.AddTransient<IBrandService, BrandService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IGoodsService, GoodsService>();
            builder.Services.AddTransient<ISpecService, SpecService>();
            builder.Services.AddTransient<ISearchService, SearchService>();
            builder.Services.AddTransient<IElasticSearchService, ElasticSearchService>();
            builder.Services.AddTransient<RedisClusterHelper>();
            builder.Services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();
            #endregion

            #region 配置文件注入
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection("ESConn"));
            builder.Services.Configure<RedisClusterOptions>(builder.Configuration.GetSection("RedisConn"));
            #endregion

            #region Consul Server IOC注册
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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

            app.MapControllers();

            app.Run();
        }
    }
}