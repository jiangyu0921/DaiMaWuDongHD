namespace DaiMaWuDong.MSACommerce.DTOModel
{
    /// <summary>
    /// DTOJWTUser   给JWT使用的
    /// </summary>
    public class DTOJWTUser
    {
        /// <summary>
        /// id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? names { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 用户类型（1：用户2：维修3：服务4：二手用户5：水果商户6：管理员）
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 密码加密的salt值
        /// </summary>
        public string? Salt { get; set; }


        /// <summary>
        /// 头像路径
        /// </summary>
        public string? avatar { get; set; }

        /// <summary>
        /// 帐号状态（0正常 1停用
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public int? del_flag { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string? login_ip { get; set; }
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        public DateTime? login_date { get; set; }

        /// <summary>
        /// 密码最后更新时间
        /// </summary>
        public DateTime? pwd_update_date { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string? create_by { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string? update_by { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? remark { get; set; }
        /// <summary>
        /// 手机标识码
        /// </summary>
        public string? scid { get; set; }
    }
}