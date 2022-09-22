using DaiMaWuDong.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model.Models
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


        //public OrangeContext(DbContextOptions<OrangeContext> options)
        //	: base(options)
        //{
        //	//this.Database.l
        //}
        public virtual DbSet<TbOrder> TbOrder { get; set; }
        public virtual DbSet<TbOrderDetail> TbOrderDetail { get; set; }
        public virtual DbSet<TbOrderStatus> TbOrderStatus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connStr, new MySqlServerVersion(new Version(5, 7, 32)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_order");

                entity.HasIndex(e => e.BuyerNick)
                    .HasDatabaseName("buyer_nick");

                entity.HasIndex(e => e.CreateTime)
                    .HasDatabaseName("create_time");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("订单id");

                entity.Property(e => e.ActualPay)
                    .HasColumnName("actual_pay")
                    .HasColumnType("bigint(20)")
                    .HasComment("实付金额。单位:分。如:20007，表示:200元7分");

                entity.Property(e => e.BuyerMessage)
                    .HasColumnName("buyer_message")
                    .HasColumnType("varchar(128)")
                    .HasComment("买家留言")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.BuyerNick)
                    .IsRequired()
                    .HasColumnName("buyer_nick")
                    .HasColumnType("varchar(32)")
                    .HasComment("买家昵称")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.BuyerRate)
                    .HasColumnName("buyer_rate")
                    .HasComment("买家是否已经评价,0未评价，1已评价");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("订单创建时间");

                entity.Property(e => e.InvoiceType)
                    .HasColumnName("invoice_type")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'")
                    .HasComment("发票类型(0无发票1普通发票，2电子发票，3增值税发票)");

                entity.Property(e => e.PaymentType)
                    .HasColumnName("payment_type")
                    .HasColumnType("tinyint(1) unsigned zerofill")
                    .HasComment("支付类型，1、在线支付，2、货到付款");

                entity.Property(e => e.PostFee)
                    .HasColumnName("post_fee")
                    .HasColumnType("bigint(20)")
                    .HasComment("邮费。单位:分。如:20007，表示:200元7分");

                entity.Property(e => e.PromotionIds)
                    .HasColumnName("promotion_ids")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.Receiver)
                    .HasColumnName("receiver")
                    .HasColumnType("varchar(32)")
                    .HasComment("收货人")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverAddress)
                    .HasColumnName("receiver_address")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（街道、住址等详细地址）")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverCity)
                    .HasColumnName("receiver_city")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（市）")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverDistrict)
                    .HasColumnName("receiver_district")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（区/县）")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverMobile)
                    .HasColumnName("receiver_mobile")
                    .HasColumnType("varchar(11)")
                    .HasComment("收货人手机")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverState)
                    .HasColumnName("receiver_state")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（省）")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ReceiverZip)
                    .HasColumnName("receiver_zip")
                    .HasColumnType("varchar(16)")
                    .HasComment("收货人邮编")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ShippingCode)
                    .HasColumnName("shipping_code")
                    .HasColumnType("varchar(20)")
                    .HasComment("物流单号")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.ShippingName)
                    .HasColumnName("shipping_name")
                    .HasColumnType("varchar(20)")
                    .HasComment("物流名称")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.SourceType)
                    .HasColumnName("source_type")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'2'")
                    .HasComment("订单来源：1:app端，2：pc端，3：M端，4：微信端，5：手机qq端");

                entity.Property(e => e.TotalPay)
                    .HasColumnName("total_pay")
                    .HasColumnType("bigint(20)")
                    .HasComment("总金额，单位为分");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(32)")
                    .HasComment("用户id")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");
            });

            modelBuilder.Entity<TbOrderDetail>(entity =>
            {
                entity.ToTable("tb_order_detail");

                entity.HasComment("订单详情表");

                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("key_order_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("订单详情id ");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''")
                    .HasComment("商品图片")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Num)
                    .HasColumnName("num")
                    .HasColumnType("int(11)")
                    .HasComment("购买数量");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("订单id");

                entity.Property(e => e.OwnSpec)
                    .HasColumnName("own_spec")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("商品动态属性键值集")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("bigint(20)")
                    .HasComment("价格,单位：分");

                entity.Property(e => e.SkuId)
                    .HasColumnName("sku_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("sku商品id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(256)")
                    .HasComment("商品标题")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbOrderStatus>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_order_status");

                entity.HasComment("订单状态表");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("status");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("订单id");

                entity.Property(e => e.CloseTime)
                    .HasColumnName("close_time")
                    .HasColumnType("datetime")
                    .HasComment("交易关闭时间");

                entity.Property(e => e.CommentTime)
                    .HasColumnName("comment_time")
                    .HasColumnType("datetime")
                    .HasComment("评价时间");

                entity.Property(e => e.ConsignTime)
                    .HasColumnName("consign_time")
                    .HasColumnType("datetime")
                    .HasComment("发货时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("订单创建时间");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("datetime")
                    .HasComment("交易完成时间");

                entity.Property(e => e.PaymentTime)
                    .HasColumnName("payment_time")
                    .HasColumnType("datetime")
                    .HasComment("付款时间");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(1)")
                    .HasComment("状态：1、未付款 2、已付款,未发货 3、已发货,未确认 4、交易成功 5、交易关闭 6、已评价");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
