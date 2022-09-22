using DaiMaWuDong.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model.Models
{
    public partial class OrangePayContext : DbContext
    {
        private readonly IOptionsMonitor<MySqlConnOptions> _optionsMonitor;
        private readonly string _connStr;

        public OrangePayContext(IOptionsMonitor<MySqlConnOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            _connStr = _optionsMonitor.CurrentValue.Url;
        }
        public virtual DbSet<TbPayLog> TbPayLog { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connStr,
                        new MySqlServerVersion(new Version(5, 7, 32)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbPayLog>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_pay_log");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("订单号");

                entity.Property(e => e.BankType)
                    .HasColumnName("bank_type")
                    .HasColumnType("varchar(16)")
                    .HasComment("银行类型")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ClosedTime)
                    .HasColumnName("closed_time")
                    .HasColumnType("datetime")
                    .HasComment("关闭时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.PayTime)
                    .HasColumnName("pay_time")
                    .HasColumnType("datetime")
                    .HasComment("支付时间");

                entity.Property(e => e.PayType)
                    .HasColumnName("pay_type")
                    .HasComment("支付方式，1 微信支付, 2 货到付款");

                entity.Property(e => e.RefundTime)
                    .HasColumnName("refund_time")
                    .HasColumnType("datetime")
                    .HasComment("退款时间");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("交易状态，1 未支付, 2已支付, 3 已退款, 4 支付错误, 5 已关闭");

                entity.Property(e => e.TotalFee)
                    .HasColumnName("total_fee")
                    .HasColumnType("bigint(20)")
                    .HasComment("支付金额（分）");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("varchar(32)")
                    .HasComment("微信交易号码")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("用户ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
