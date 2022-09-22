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
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                loggingBuilder.AddLog4Net();
            });
            #endregion

            #region ServiceRegister

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            //֧��Token
            #region Swagger����֧��Token�������� 

            builder.Services.AddSwaggerGen(options =>
            {

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

            });

            #endregion

            #region ����ע��
            builder.Services.AddTransient<OrangeContext, OrangeContext>();
            builder.Services.AddTransient<CacheClientDB, CacheClientDB>();
            builder.Services.AddTransient<IUserService, UserService>();
            //builder.Services.AddTransient<ISqlSugarCompanyService, SqlSugarCompanyService>();
            builder.Services.AddTransient<RedisClusterHelper>();
            #endregion

            #region �����ļ�ע��
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisConnOptions>(builder.Configuration.GetSection("RedisConn"));
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

            #region Consul Server IOCע��
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

            #region Consulע��
            app.UseHealthCheckMiddleware("/Health");//����������Ӧ
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.UseHttpsRedirection();

            #region ��Ӽ�Ȩ
            app.UseAuthentication();
            #endregion

            app.UseAuthorization();

            #region ����ȡ���������ṩ����
            //app.UseCors("default");
            #endregion

            app.MapControllers();

            app.Run();

            #endregion
        }
    }
}