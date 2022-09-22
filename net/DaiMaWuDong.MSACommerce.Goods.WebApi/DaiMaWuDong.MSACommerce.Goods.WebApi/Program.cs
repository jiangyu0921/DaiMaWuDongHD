using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DaiMaWuDong.AgileFramework.AuthenticationCenter;
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.PollyExtend;
using DaiMaWuDong.MSACommerce.Goods.Interface;
using DaiMaWuDong.MSACommerce.Goods.Model.Models;
using DaiMaWuDong.MSACommerce.Goods.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace DaiMaWuDong.MSACommerce.Goods.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region ע��HttpInvoker����
            //builder.Services.AddHttpInvoker(options =>
            //{
            //    options.Message = "This is Program's Message";
            //});
            #endregion

            #region ʹ��Autofac
            {
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>((context, buider) =>
                {
                    // ����ʹ�õ���ע��
                    buider.RegisterType<BrandService>()
                    .As<IBrandService>().SingleInstance().EnableInterfaceInterceptors();
                    buider.RegisterType<PollyPolicyAttribute>();

                });
            }
            #endregion

            builder.Services.AddTransient<AbstractConsulDispatcher, PollingDispatcher>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            #region ֧��Token
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

            #region ���Log4Net֧��
            //builder.Logging
            //    .AddFilter("System", LogLevel.Warning)
            //    .AddFilter("Microsoft", LogLevel.Warning)
            //    .AddLog4Net();
            #endregion

            #region ����ע��
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<OrangeContext>();
            builder.Services.AddTransient<RedisClusterHelper>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IBrandService, BrandService>();
            builder.Services.AddTransient<IGoodsService, GoodsService>();
            #endregion

            #region Consul Server IOCע��
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion

            #region �����ļ�ע��
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisClusterOptions>(builder.Configuration.GetSection("RedisConn"));
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