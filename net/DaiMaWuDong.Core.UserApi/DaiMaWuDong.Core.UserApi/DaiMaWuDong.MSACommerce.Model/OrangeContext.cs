using DaiMaWuDong.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model
{
    public partial class OrangeContext : DbContext
    {
        private readonly IOptionsMonitor<MySqlConnOptions> _optionsMonitor;
        private readonly string _connStr;

        public OrangeContext(IOptionsMonitor<MySqlConnOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            _connStr = _optionsMonitor.CurrentValue.Url;
        }



        public OrangeContext(string connstr)
        {
            _connStr = connstr;
        }

        //public OrangeContext(DbContextOptions<OrangeContext> options)
        //	: base(options)
        //{
        //	//this.Database.l
        //}

        #region 未使用

        //public virtual DbSet<Cid3> Cid3 { get; set; }



        ////public virtual DbSet<TbPayLog> TbPayLog { get; set; }







        #endregion


        #region 用户信息

        public virtual DbSet<TbUser> TbUser { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(this._connStr, new MySqlServerVersion(new Version(5, 7, 32)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region 用户信息 

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.ToTable("tb_user");

                entity.HasComment("用户表");

                entity.HasIndex(e => e.Username)
                    .HasName("username")
                    .IsUnique();

                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(32)")
                    .HasComment("密码，加密存储")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(11)")
                    .HasComment("注册手机号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnName("salt")
                    .HasColumnType("varchar(32)")
                    .HasComment("密码加密的salt值")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(32)")
                    .HasComment("用户名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                entity.Property(e => e.names)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(50)")
                    .HasComment("用户昵称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                entity.Property(e => e.type)
                    .HasColumnName("type")
                    .HasColumnType("int(32)");
                entity.Property(e => e.avatar)
                    .IsRequired()
                    .HasColumnName("avatar")
                    .HasColumnType("varchar(100)")
                    .HasComment("头像路径")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                 entity.Property(e => e.status)
                    .HasColumnName("status")
                    .HasColumnType("int(32)");
                 entity.Property(e => e.del_flag)
                    .HasColumnName("del_flag")
                    .HasColumnType("int(32)");
                entity.Property(e => e.login_ip)
                    .IsRequired()
                    .HasColumnName("login_ip")
                    .HasColumnType("varchar(128)")
                    .HasComment("最后登录IP")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                entity.Property(e => e.login_date)
                    .HasColumnName("login_date")
                    .HasColumnType("datetime")
                    .HasComment("更新最后登录时间");
                entity.Property(e => e.pwd_update_date)
                    .HasColumnName("pwd_update_date")
                    .HasColumnType("datetime")
                    .HasComment("密码最后更新时间");
                entity.Property(e => e.create_by)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("varchar(32)")
                    .HasComment("创建者")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                entity.Property(e => e.create_time)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("创建时间");
                entity.Property(e => e.update_by)
                    .IsRequired()
                    .HasColumnName("update_by")
                    .HasColumnType("varchar(32)")
                    .HasComment("更新者")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
                entity.Property(e => e.update_time)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime")
                    .HasComment("修改时间");
                entity.Property(e => e.remark)
                    .IsRequired()
                    .HasColumnName("remark")
                    .HasColumnType("varchar(500)")
                    .HasComment("备注")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci"); 
                entity.Property(e => e.scid)
                    .IsRequired()
                    .HasColumnName("scid")
                    .HasColumnType("varchar(50)")
                    .HasComment("手机标识码")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
