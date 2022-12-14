using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Interface
{
    public interface IUserService
    {
        /**
    * 校验用户对象数据类型
    * @param data
    * @param type
    * @return
    */
        AjaxResult CheckData(string data, int type);

        /**
         * 发送验证码
         * @param phone
         */
        AjaxResult SendVerifyCode(string phone);

        /// <summary>
        /// 发送验证码前验证
        /// </summary>
        /// <param name="phone"></param>
        AjaxResult CheckPhoneNumberBeforeSend(string phone);

        /**
         * 用户注册
         * @param user
         * @param code
         */
        AjaxResult Register(TbUser user, string code);

        /**
         * 根据账号和密码查询用户信息
         * @param username
         * @param password
         * @return
         */
        AjaxResult<TbUser> QueryUser(string username, string password, int type);

        /**
        * 根据用户名和key值验证信息
        * @param username
        * @param password
        * @return
        */
        AjaxResult<bool> QueryUserPow(string username, string verificationPow);


        /**
        * 修改密码
        * @param username
        * @param oldPassword
        * @param NewPassword
        * @return
        */
        AjaxResult ModifyPow(string username, string oldPassword, string NewPassword);

        /**
        * 修改信息
        * @param username
        * @param oldPassword
        * @param NewPassword
        * @return
        */
        AjaxResult ModifyDate(TbUser user);
    }
}
