using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.AgileFramework.Encryption;
using DaiMaWuDong.AgileFramework.Message;
using DaiMaWuDong.AgileFramework.ORMSelfResearch;
using DaiMaWuDong.AgileFramework.ORMSelfResearch.Services;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Service
{
    public class UserService : IUserService
    {
        private CacheClientDB _cacheClientDB;
        private readonly IOptionsMonitor<MySqlConnOptions> _optionsMonitor;
        private static DbProxyCoreOptions dbProxyCoreOptions;
        public UserService(CacheClientDB cacheClientDB, IOptionsMonitor<MySqlConnOptions> optionsMonitor)//, ISqlSugarCompanyService sqlSugarCompanyService)
        {
            _cacheClientDB = cacheClientDB;
            _optionsMonitor = optionsMonitor;
            dbProxyCoreOptions = new CustomDbProxy(new DbProxyOptions()
            {
                ConnectionString = _optionsMonitor.CurrentValue.Url
            });
            //_sqlSugarCompanyService = sqlSugarCompanyService;
        }
        private static readonly string KEY_PREFIX = "user:verify:code:";



        /// <summary>
        /// 检查数据重复
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type">1 账号  2手机号</param>
        /// <returns></returns>
        public AjaxResult CheckData(string data, int type)
        {
            int exist = 0;
            //判断校验数据的类型
            switch (type)
            {
                case 1:
                    //exist = _orangeContext.TbUser.Count(u => u.Username.Equals(data));

                    exist = dbProxyCoreOptions.Query<TbUser>(c => c.Username == data && c.status == 0 && c.del_flag == 0).Count;
                    return new AjaxResult()
                    {
                        Result = exist == 0,
                        Message = exist == 0 ? "校验成功" : "校验失败，用户名重复"
                    };
                case 2:
                    //exist = _orangeContext.TbUser.Count(u => u.Phone.Equals(data));
                    exist = dbProxyCoreOptions.Query<TbUser>(c => c.Phone == data && c.status == 0 && c.del_flag == 0).Count;
                    return new AjaxResult()
                    {
                        Result = exist == 0,
                        Message = exist == 0 ? "校验成功" : "校验失败，手机号重复"
                    };
                default:
                    return new AjaxResult()
                    {
                        Result = false,
                        Message = "参数type不合法，校验未通过"
                    };
            }
        }
        /// <summary>
        /// 根据账号密码查询用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AjaxResult<TbUser> QueryUser(string username, string password, int type)
        {
            switch (type)
            {
                case 1:
                    return QueryUserpows(username, password);
                case 2:
                    return QueryUserOnekey(username);
                case 3:
                    return QueryUserSendVerify(username, password);
                default:
                    return QueryUserpows(username, password);
            }
        }

        /// <summary>
        /// 短信登录
        /// </summary>
        /// <param name="username">手机号</param>
        /// <param name="password">验证码</param>
        /// <returns></returns>
        private AjaxResult<TbUser> QueryUserSendVerify(string username, string password)
        {
            string key = KEY_PREFIX + username;
            lock (Redis_Lock)//单线程，避免重复提交
            {
                string value = _cacheClientDB.Get<string>(key);
                if (!password.Equals(value))
                {
                    return new AjaxResult<TbUser>()
                    {
                        Result = false,
                        TValue = new TbUser(),
                        Message = "验证码不匹配"
                    };
                    //验证码不匹配
                    //throw new Exception("验证码不匹配");
                }
                _cacheClientDB.Remove(key);//把验证码从Redis中删除
            }
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Username == username && c.status == 0 && c.del_flag == 0);

            if (users == null || users.Count < 1)
            {
                //throw new Exception("查询的用户不存在！");
                return new AjaxResult<TbUser>()
                {
                    Result = false,
                    TValue = new TbUser(),
                    Message = "查询的用户不存在!"
                };
            }
            TbUser user = users.FirstOrDefault()!;
            return new AjaxResult<TbUser>()
            {
                Result = true,
                TValue = user,
                Message = "成功"
            };
        }

        /// <summary>
        /// 一键登录
        /// </summary>
        /// <param name="username">手机号</param>
        /// <returns></returns>
        private AjaxResult<TbUser> QueryUserOnekey(string username)
        {
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Phone == username && c.status == 0 && c.del_flag == 0);
            if (users == null || users.Count < 1)
            {
                var ajaxResult = Register(new TbUser()
                {
                    Phone = username,

                }, "一键登录");
                if (!ajaxResult.Result)
                {
                    return new AjaxResult<TbUser>()
                    {
                        Result = false,
                        TValue = new TbUser(),
                        Message = ajaxResult.Message
                    };
                }
                var user = dbProxyCoreOptions.Query<TbUser>(c => c.Username == username && c.status == 0 && c.del_flag == 0);
                if (user == null || user.Count < 1)
                {
                    //throw new Exception("查询的用户不存在！");
                    return new AjaxResult<TbUser>()
                    {
                        Result = false,
                        TValue = new TbUser(),
                        Message = "查询的用户不存在!"
                    };
                }
                return new AjaxResult<TbUser>()
                {
                    Result = true,
                    TValue = user.FirstOrDefault()!,
                    Message = "成功"
                };
            }
            return new AjaxResult<TbUser>()
            {
                Result = true,
                TValue = users.FirstOrDefault()!,
                Message = "成功"
            };
        }

        /// <summary>
        /// 根据账号密码查询用户
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private AjaxResult<TbUser> QueryUserpows(string username, string password)
        {
            //首先根据用户名查询用户
            //TbUser user = _orangeContext.TbUser.Where(m => m.Username == username).FirstOrDefault();
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Username == username && c.status == 0 && c.del_flag == 0);

            if (users == null || users.Count < 1)
            {
                //throw new Exception("查询的用户不存在！");
                return new AjaxResult<TbUser>()
                {
                    Result = false,
                    TValue = new TbUser(),
                    Message = "查询的用户不存在!"
                };
            }
            TbUser user = users.FirstOrDefault()!;

            if (MD5Helper.MD5EncodingWithSalt(password, user.Salt) != user.Password)
            {
                //密码不正确
                //throw new Exception("密码错误");
                return new AjaxResult<TbUser>()
                {
                    Result = false,
                    TValue = new TbUser(),
                    Message = "密码错误"
                };
            }
            user.login_date = DateTime.Now;
            user.login_ip = GetIp();
            dbProxyCoreOptions.Update(user);
            return new AjaxResult<TbUser>()
            {
                Result = true,
                TValue = user,
                Message = "成功"
            };
        }
        /// <summary>
        /// 根据用户名和key值验证信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AjaxResult<bool> QueryUserPow(string username, string verificationPow)
        {
            //首先根据用户名查询用户
            //TbUser user = _orangeContext.TbUser.Where(m => m.Username == username).FirstOrDefault();

            //TbUser user = _sqlSugarCompanyService.Query<TbUser>().Where(t=>t.Username== username).First();
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Username == username && c.status == 0 && c.del_flag == 0);
            if (users == null)
            {
                //throw new Exception("查询的用户不存在！");
                return new AjaxResult<bool>()
                {
                    Result = false,
                    TValue = false,
                    Message = "查询的用户不存在!"
                };
            }
            TbUser user = users.FirstOrDefault()!;

            if (MD5Helper.MD5EncodingWithSalt(user.Password, username) != verificationPow)
            {
                //密码不正确
                //throw new Exception("密码错误");
                return new AjaxResult<bool>()
                {
                    Result = false,
                    TValue = false,
                    Message = "密码错误"
                };
            }
            return new AjaxResult<bool>()
            {
                Result = true,
                TValue = true,
                Message = "成功"
            };
        }
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        public AjaxResult Register(TbUser user, string code)
        {
            if (code == "一键登录")
            {
                user.Username = user.Phone;
                user.Password = "jnfu.123";
            }
            else
            {
                string key = KEY_PREFIX + user.Phone;
                lock (Redis_Lock)//单线程，避免重复提交
                {
                    string value = _cacheClientDB.Get<string>(key);
                    if (!code.Equals(value))
                    {
                        return new AjaxResult<bool>()
                        {
                            Result = false,
                            Message = "验证码不匹配"
                        };
                        //验证码不匹配
                        //throw new Exception("验证码不匹配");
                    }
                    _cacheClientDB.Remove(key);//把验证码从Redis中删除
                }
            }
            user.Salt = MD5Helper.MD5EncodingOnly(user.Username);
            string md5Pwd = MD5Helper.MD5EncodingWithSalt(user.Password, user.Salt);
            user.Password = md5Pwd;
            user.status = 0;
            user.del_flag = 0;
            Random random = new Random();
            user.names = "用户" + random.Next(100000, 999999).ToString();
            //_orangeContext.Add(user);
            //int count = _orangeContext.SaveChanges();
            if (!dbProxyCoreOptions.Insert(user))
            {
                //throw new Exception("用户注册失败");
                return new AjaxResult<bool>()
                {
                    Result = true,
                    Message = "用户注册失败"
                };
            }
            return new AjaxResult<bool>()
            {
                Result = true,
                Message = "成功"
            };
        }

        private static readonly object Redis_Lock = new object();


        /// <summary>
        /// 发送验证码方法
        /// </summary>
        /// <param name="phone"></param>
        public AjaxResult SendVerifyCode(string phone)
        {
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();// 生成随机6位数字验证码
            string key = KEY_PREFIX + phone;
            _cacheClientDB.Set(key, code, TimeSpan.FromMinutes(5));// 把验证码存储到redis中  5分钟有效,有则覆盖
            _cacheClientDB.Set(key + "1m1t", code, TimeSpan.FromMinutes(1));//一分钟只能发一次

            return SMSTool.SendValidateCode(phone, code);// 调用发送短信的方法
        }

        /// <summary>
        /// 1  数据库不存在
        /// 2  Redis注册频次
        /// 3  该号码一天多少次短信
        /// 4  该IP一天多少次短信
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public AjaxResult CheckPhoneNumberBeforeSend(string phone)
        {
            //var list = this._orangeContext.TbUser.Where(u => u.Phone.Equals(phone)).ToList();
            var list = dbProxyCoreOptions.Query<TbUser>(c => c.Phone == phone && c.status == 0 && c.del_flag == 0).ToList();
            if (list.Count > 0)
            {
                return new AjaxResult()
                {
                    Result = false,
                    Message = "手机号码重复"
                };
            }

            string key = KEY_PREFIX + phone;
            if (!string.IsNullOrWhiteSpace(_cacheClientDB.Get<string>(key + "1m1t")))
            {
                return new AjaxResult()
                {
                    Result = false,
                    Message = "1分钟内只能发一次验证码"
                };
            }

            return new AjaxResult()
            {
                Result = true,
                Message = "发送成功"
            };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldPassword">原密码</param>
        /// <param name="NewPassword">新密码</param>
        /// <returns></returns>
        public AjaxResult ModifyPow(string username, string oldPassword, string NewPassword)
        {
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Username == username && c.status == 0 && c.del_flag == 0);
            if (users == null || users.Count < 1)
            {
                //throw new Exception("查询的用户不存在！");
                return new AjaxResult()
                {
                    Result = false,
                    Message = "查询的用户不存在!"
                };
            }
            TbUser user = users.FirstOrDefault()!;

            if (MD5Helper.MD5EncodingWithSalt(oldPassword, user.Salt) != user.Password)
            {
                //密码不正确
                //throw new Exception("密码错误");
                return new AjaxResult()
                {
                    Result = false,
                    Message = "密码错误"
                };
            }
            string md5Pwd = MD5Helper.MD5EncodingWithSalt(NewPassword, user.Salt);
            user.Password = md5Pwd;
            user.pwd_update_date = DateTime.Now;
            var result = dbProxyCoreOptions.Update(user);
            if (!result)
            {
                return new AjaxResult()
                {
                    Result = false,
                    Message = "修改密码失败!"
                };
            }
            return new AjaxResult()
            {
                Result = false,
                Message = "修改密码成功!"
            };
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AjaxResult ModifyDate(TbUser user)
        {
            var users = dbProxyCoreOptions.Query<TbUser>(c => c.Username == user.Username && c.status == 0 && c.del_flag == 0);
            if (users == null || users.Count < 1)
            {
                //throw new Exception("查询的用户不存在！");
                return new AjaxResult()
                {
                    Result = false,
                    Message = "查询的用户不存在!"
                };
            }
            TbUser model = users.FirstOrDefault()!;
            user.Password = model.Password;
            var result = dbProxyCoreOptions.Update(user);
            if (!result)
            {
                return new AjaxResult()
                {
                    Result = false,
                    Message = "修改密码失败!"
                };
            }
            return new AjaxResult()
            {
                Result = false,
                Message = "修改密码成功!"
            };
        }

        private string GetIp()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                var addressV = iPHostEntry.AddressList.FirstOrDefault(q => q.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);//ip4地址
                if (addressV != null)
                    return addressV.ToString();
                return "127.0.0.1";
            }
            catch (Exception ex)
            {
                return "127.0.0.1";
            }
        }
    }
}
