
using DaiMaWuDong.AgileFramework.AuthenticationCenter.JWTExtend;
using DaiMaWuDong.AgileFramework.Consul;
using DaiMaWuDong.AgileFramework.Encryption;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.AuthenticationCenter.Utility;
using DaiMaWuDong.MSACommerce.DTOModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DaiMaWuDong.MSACommerce.AuthenticationCenter
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region �ӿڵ���
            builder.Services.AddTransient<HttpHelperService>();
            #endregion

            //��ҪJWT�ķ�װ
            #region JWT
            #region HS256 �Գƿ������
            //builder.Services.AddScoped<IJWTService, JWTHSService>();
            //builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

            //���˸�Extension��װ��������ע��ͳ�ʼ��ϸ��
            builder.Services.AddJWTBuilder(JWTAlgorithmType.HS256, () =>
            {
                builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
            });
            #endregion

            #region RS256 �ǶԳƿ�����ܣ���Ҫ��ȡһ�ι�Կ
            ////string keyDir = Directory.GetCurrentDirectory();
            ////if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            ////{
            ////    keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            ////}

            ////builder.Services.AddScoped<IJWTService, JWTRSService>();
            ////builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

            //builder.Services.AddJWTBuilder(JWTAlgorithmType.HS256, () =>
            //{
            //    builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
            //});
            #endregion
            #endregion


            #region Consul Server IOCע��
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion

            #region IHttpContextAccessor
            builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
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



            app.UseAuthorization();

            #region ��ȡToken


            app.MapPost("/api/auth/accredit", (
                ILogger<Program> logger,
                [FromServices] IJWTService jwtService,
                HttpHelperService httpHelperService,
                //[FromServices] AbstractConsulDispatcher consulDispatcher,//IOCע��
                IHttpContextAccessor httpContextAccessor,
                [FromBody] UserIdClass user
                ) =>
            {
                //Console.WriteLine($"{username}--{password}");
                var httpContext = httpContextAccessor.HttpContext;

                //var Response= httpContext.Request.Form;
                //if (string.IsNullOrEmpty(username))
                //{
                //    username = Response["username"];
                //}
                //if (string.IsNullOrEmpty(password))
                //{
                //    password = Response["password"];
                //}

                string requestUrl = $"{builder.Configuration["VerifyUserUrl"]}?username={user.username}&password={user.password}";
                string realUrl = requestUrl;//this._IConsulDispatcher.GetAddress(requestUrl);

                Console.WriteLine($"{requestUrl}--{realUrl}");
                AjaxResult<DTOJWTUser> ajaxResult = httpHelperService.VerifyUser<DTOJWTUser>(realUrl);
                if (ajaxResult.Result)
                {
                    var dtoUser = ajaxResult.TValue;

                    string token = jwtService.GetToken(new JWTUserModel()
                    {
                        id = dtoUser.id,
                        username = dtoUser.username
                    });
                    ajaxResult.Value = token;
                }
                Console.WriteLine($"Accredit Result : {JsonConvert.SerializeObject(ajaxResult)}");

                return ajaxResult;

                //return new JsonResult(ajaxResult);//�����ᱻ���һ��
            });

            #endregion

            #region ��ȡrefreshToken
            app.MapPost("/api/auth/accreditWithRefresh",
                (ILogger<Program> logger,
                IJWTService jwtService,
                HttpHelperService httpHelperService,
                //[FromServices] AbstractConsulDispatcher consulDispatcher,//IOCע��
                IHttpContextAccessor httpContextAccessor,
                [FromBody] UserIdClass user
                )
                =>
                {
                    Console.WriteLine($"This is LoginWithRefresh {user.username}--{user.password}");
                    var httpContext = httpContextAccessor.HttpContext;
                    //var Response = httpContext.Request.Form;
                    //if (string.IsNullOrEmpty(user.username))
                    //{
                    //    username = Response["username"];
                    //}
                    //if (string.IsNullOrEmpty(password))
                    //{
                    //    password = Response["password"];
                    //}

                    string requestUrl = $"{builder.Configuration["VerifyUserUrl"]}?username={user.username}&password={user.password}";
                    string realUrl = requestUrl;//this._IConsulDispatcher.GetAddress(requestUrl);

                    //Console.WriteLine($"{requestUrl}--{realUrl}");
                    AjaxResult<DTOJWTUser> ajaxResult = httpHelperService.VerifyUser<DTOJWTUser>(realUrl);
                    if (ajaxResult.Result)
                    {
                        var dtoUser = ajaxResult.TValue;

                        var tokenPair = jwtService.GetTokenWithRefresh(new JWTUserModel()
                        {
                            id = dtoUser.id,
                            username = dtoUser.username,
                            verificationPow = MD5Helper.MD5EncodingWithSalt(dtoUser.password, dtoUser.username)
                        });
                        if (tokenPair != null && !string.IsNullOrEmpty(tokenPair.Item1))
                        {
                            ajaxResult.Value = tokenPair.Item1;
                            ajaxResult.OtherValue = tokenPair.Item2;
                        }
                        else
                        {
                            ajaxResult.Result = false;
                            ajaxResult.Message = "�䷢tokenʧ��";
                        }
                    }
                    Console.WriteLine($"Accredit Result : {JsonConvert.SerializeObject(ajaxResult)}");
                    return ajaxResult;
                });

            app.MapPost("/api/auth/refresh",
                 (ILogger<Program> logger,
                 IJWTService jwtService,
                 IHttpContextAccessor httpContextAccessor,
                HttpHelperService httpHelperService,
                 string? refreshToken
                 )
                 =>
                 {

                     var httpContext = httpContextAccessor.HttpContext;
                     var Response = httpContext.Request.Form;
                     if (string.IsNullOrEmpty(refreshToken))
                     {
                         refreshToken = Response["refreshToken"];
                     }
                     Console.WriteLine($"This is refresh {refreshToken}");
                     string result = refreshToken.AnalysisToken();
                     if (string.IsNullOrEmpty(result))
                     {
                         return new AjaxResult()
                         {
                             Result = false,
                             Message = "��refreshTokenֵ"
                         };
                     }
                     var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                     string requestUrl = $"{builder.Configuration["VerifyUserPowUrl"]}?username={jsonResult[ClaimTypes.Name]}&verificationPow={jsonResult["verificationPow"]}";
                     string realUrl = requestUrl;
                     AjaxResult<bool> ajaxResult = httpHelperService.VerifyUser<bool>(realUrl);
                     if (!ajaxResult.TValue)
                     {
                         return new AjaxResult()
                         {
                             Result = false,
                             Message = "�û������Ѿ��޸�"
                         };
                     }

                     if (!jsonResult.ValidateRefreshToken())
                     {
                         return new AjaxResult()
                         {
                             Result = false,
                             Message = "refreshToken������"
                         };
                     }
                     else
                     {
                         var token = jwtService.GetTokenByRefresh(refreshToken);
                         return new AjaxResult()
                         {
                             Result = true,
                             Value = token
                         };
                     }
                 });
            #endregion


            #region Consulע��
            //app.UseHealthCheckMiddleware("/Health");//����������Ӧ
            //app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.MapControllers();

            #endregion
            app.Run();
        }
    }
    public class UserIdClass
    {
        public string? username
        {
            get; set;
        }
        public string? password { get; set; }
    }

}