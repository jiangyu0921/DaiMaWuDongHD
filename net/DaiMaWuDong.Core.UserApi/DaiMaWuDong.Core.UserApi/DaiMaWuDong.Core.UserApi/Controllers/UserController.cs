
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DaiMaWuDong.Core.UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Identity
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        //private readonly IPLocation.IPLocationClient _IPLocationClient;//后面gRPC提供IP定位功能
        //private readonly AbstractConsulDispatcher _AbstractConsulDispatcher = null;//Consul等会儿
        private readonly IConfiguration _IConfiguration = null;
        public UserController(IUserService userService, ILogger<UserController> logger,
            //IPLocation.IPLocationClient ipLocationClient, 
            //AbstractConsulDispatcher abstractConsulDispatcher, 
            IConfiguration configuration)
        {
            this._userService = userService;
            this._logger = logger;
            //this._IPLocationClient = ipLocationClient;
            //this._AbstractConsulDispatcher = abstractConsulDispatcher;
            this._IConfiguration = configuration;
        }
        #endregion

        /// <summary>
        /// 检查信息格式
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("check/{data}/{type}")]
        [HttpGet]
        public JsonResult CheckData(string data, int type)
        {
            return new JsonResult(_userService.CheckData(data, type));
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("send")]
        [HttpPost]
        public JsonResult SendVerifyCode([FromForm] string phone)
        {
            //检查的时候，需要 ip--

            AjaxResult ajaxResult = this._userService.CheckPhoneNumberBeforeSend(phone);
            if (!ajaxResult.Result)//校验失败
            {
                return new JsonResult(ajaxResult);
            }
            else
            {

                return new JsonResult(_userService.SendVerifyCode(phone));
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        //[TypeFilter(typeof(CustomAction2CommitFilterAttribute))]
        public JsonResult Register([FromForm] TbUser user, [FromForm] string code)
        {
            _userService.Register(user, code);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "注册成功"
            });
        }

        /// <summary>
        /// 根据用户名和密码查询用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("query")]
        [HttpGet]
        public JsonResult QueryUser(string username, string password)
        {
            Console.WriteLine($"This is {typeof(UserController).Name}{nameof(QueryUser)} username={username} password={password}");
            AjaxResult<TbUser> ajaxResult = null;
            TbUser tbUser = _userService.QueryUser(username, password);

            ajaxResult = new AjaxResult<TbUser>()
            {
                Result = true,
                TValue = tbUser
            };
            _logger.LogInformation($"用户微服务->用户的登录操作 username={username} password={password}");
            return new JsonResult(ajaxResult);
        }

        /// <summary>
        /// 根据用户名和key值验证信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("queryPow")]
        [HttpGet]
        public JsonResult QueryUserPow(string username, string verificationPow)
        {
            Console.WriteLine($"This is {typeof(UserController).Name}{nameof(QueryUser)} verificationPow={verificationPow}");
            AjaxResult<bool> ajaxResult = null;
            bool result = _userService.QueryUserPow(username, verificationPow);

            ajaxResult = new AjaxResult<bool>()
            {
                Result = true,
                TValue = result
            };
            return new JsonResult(ajaxResult);
        }

        /**
	    * 根据用户名和密码查询用户
	    * @param username
	    * @param password
	    * @return
	    */
        [Route("/api/user/verify")]
        [HttpGet]
        [AllowAnonymousAttribute]//自己校验
        public JsonResult CurrentUser()
        {
            //if (new Random().Next(1, 100) > 50)
            AjaxResult ajaxResult = null;
            IEnumerable<Claim> claimlist = HttpContext.AuthenticateAsync()?.Result?.Principal?.Claims;
            if (claimlist != null && claimlist.Count() > 0)
            {
                DateTime expiredTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                expiredTime = expiredTime.AddSeconds(int.Parse(claimlist.FirstOrDefault(u => u.Type == "exp").Value));
                if (expiredTime >= DateTime.Now)
                {
                    string username = claimlist.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
                    string id = claimlist.FirstOrDefault(u => u.Type == "Id").Value;
                    ajaxResult = new AjaxResult()
                    {
                        StatusCode = 200,
                        Result = true,
                        Value = new
                        {
                            id = id,
                            username = username,
                        }
                    };
                }
                else
                {
                    ajaxResult = new AjaxResult()
                    {
                        StatusCode = 401,
                        Result = false,
                        Message = "Token无效,请从新获取"
                    };
                }
            }
            else
            {
                ajaxResult = new AjaxResult()
                {
                    StatusCode = 401,
                    Result = false,
                    Message = "Token无效，请重新登陆"
                };
            }
            return new JsonResult(ajaxResult);
        }

        [Route("/api/user/getwithauthorize")]
        [HttpGet]
        [Authorize]//要求授权
        public JsonResult GetWithAuthorize()
        {
            return new JsonResult(new AjaxResult()
            {
                Value = base.HttpContext.User.Identity?.Name
            });
        }

        #region gRPC  IpLocation
        //[Route("/api/user/location")]
        //[HttpGet]
        //[AllowAnonymousAttribute]
        //public JsonResult CurrentLocation()
        //{
        //    //string targetUrl = $"http://{this._IConsulDispatcher.ChooseAddress("LessonService")}";
        //    ////"http://localhost:8000"

        //    {
        //        //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        //        //LocationReply locationReply = null;
        //        //locationReply = this._IPLocationClient.Location(new IPRequest() { Ip = base.HttpContext.Connection.RemoteIpAddress.ToString() });
        //        //return new JsonResult(new AjaxResult()
        //        //{
        //        //    Result = true,
        //        //    Value = locationReply.LocationDetail
        //        //});
        //    }
        //    {
        //        string targetUrl = this._AbstractConsulDispatcher.GetAddress(this._IConfiguration["IPLibraryServiceUrl"]);
        //        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        //        using (var channel = GrpcChannel.ForAddress(targetUrl))
        //        {
        //            var client = new IPLocation.IPLocationClient(channel);
        //            {
        //                var locationReply = client.Location(new IPRequest() { Ip = base.HttpContext.Connection.RemoteIpAddress.ToString() });
        //                return new JsonResult(new AjaxResult()
        //                {
        //                    Result = true,
        //                    Value = locationReply.LocationDetail
        //                });
        //            }
        //        }
        //    }
        //}
        #endregion

    }
}
