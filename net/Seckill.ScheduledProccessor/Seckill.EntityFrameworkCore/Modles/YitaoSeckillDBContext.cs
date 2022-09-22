using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.EntityFrameworkCore.Modles
{
    /// <summary>
    /// 秒杀项目操作Context类
    /// </summary>
    public partial class YitaoSeckillDBContext : DbContext
    {
        public YitaoSeckillDBContext()
        {
        }

        public YitaoSeckillDBContext(DbContextOptions<YitaoSeckillDBContext> options) : base(options)
        {
        }

        public virtual DbSet<TbSeckillGood> TbSeckillGoods { get; set; }
        public virtual DbSet<TbSeckillOrder> TbSeckillOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=192.168.3.254;database=yitao_seckill;user=root;password=123;treattinyasboolean=true", ServerVersion.Parse("8.0.21-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<TbSeckillGood>(entity =>
            {
                entity.ToTable("tb_seckill_goods");

                entity.UseCollation("utf8mb4_bin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CheckTime)
                    .HasColumnType("datetime")
                    .HasColumnName("check_time")
                    .HasComment("审核日期");

                entity.Property(e => e.CostPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("cost_price")
                    .HasComment("秒杀价格");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("添加日期");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time")
                    .HasComment("结束时间");

                entity.Property(e => e.Introduction)
                    .HasMaxLength(2000)
                    .HasColumnName("introduction")
                    .HasComment("描述");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name")
                    .HasComment("标题");

                entity.Property(e => e.Num)
                    .HasColumnName("num")
                    .HasComment("秒杀商品数");

                entity.Property(e => e.Price)
                    .HasPrecision(10, 2)
                    .HasColumnName("price")
                    .HasComment("原价格");

                entity.Property(e => e.SkuId)
                    .HasColumnName("sku_id")
                    .HasComment("sku ID");

                entity.Property(e => e.SmallPic)
                    .HasMaxLength(150)
                    .HasColumnName("small_pic")
                    .HasComment("商品图片");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time")
                    .HasComment("开始时间");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .HasColumnName("status")
                    .IsFixedLength(true)
                    .HasComment("审核状态，0未审核，1审核通过，2审核不通过");

                entity.Property(e => e.StockCount)
                    .HasColumnName("stock_count")
                    .HasComment("剩余库存数");

                entity.Property(e => e.SpuId)
                    .HasColumnName("spu_id")
                    .HasComment("spu ID");
            });

            modelBuilder.Entity<TbSeckillOrder>(entity =>
            {
                entity.ToTable("tb_seckill_order");

                entity.UseCollation("utf8mb4_bin");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id")
                    .HasComment("主键");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.Money)
                    .HasPrecision(10, 2)
                    .HasColumnName("money")
                    .HasComment("支付金额");

                entity.Property(e => e.PayTime)
                    .HasColumnType("datetime")
                    .HasColumnName("pay_time")
                    .HasComment("支付时间");

                entity.Property(e => e.Receiver)
                    .HasMaxLength(20)
                    .HasColumnName("receiver")
                    .HasComment("收货人");

                entity.Property(e => e.ReceiverAddress)
                    .HasMaxLength(200)
                    .HasColumnName("receiver_address")
                    .HasComment("收货人地址");

                entity.Property(e => e.ReceiverMobile)
                    .HasMaxLength(20)
                    .HasColumnName("receiver_mobile")
                    .HasComment("收货人电话");

                entity.Property(e => e.SeckillId)
                    .HasColumnName("seckill_id")
                    .HasComment("秒杀商品ID");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .HasColumnName("status")
                    .IsFixedLength(true)
                    .HasComment("状态，0未支付，1已支付");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(30)
                    .HasColumnName("transaction_id")
                    .HasComment("交易流水");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id")
                    .HasComment("用户");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
