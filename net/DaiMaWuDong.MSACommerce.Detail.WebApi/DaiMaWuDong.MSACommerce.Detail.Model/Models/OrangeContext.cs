using DaiMaWuDong.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Detail.Model.Models
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
        public virtual DbSet<TbSpu> TbSpu { get; set; }
        public virtual DbSet<TbSpuDetail> TbSpuDetail { get; set; }
        public virtual DbSet<TbSku> TbSku { get; set; }

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
            modelBuilder.Entity<TbSpu>(entity =>
            {
                entity.ToTable("tb_spu");

                entity.HasComment("spu表，该表描述的是一个抽象性的商品，比如 iphone8");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("spu id");

                entity.Property(e => e.BrandId)
                    .HasColumnName("brand_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("商品所属品牌id");

                entity.Property(e => e.Cid1)
                    .HasColumnName("cid1")
                    .HasColumnType("bigint(20)")
                    .HasComment("1级类目id");

                entity.Property(e => e.Cid2)
                    .HasColumnName("cid2")
                    .HasColumnType("bigint(20)")
                    .HasComment("2级类目id");

                entity.Property(e => e.Cid3)
                    .HasColumnName("cid3")
                    .HasColumnType("bigint(20)")
                    .HasComment("3级类目id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("添加时间");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnName("last_update_time")
                    .HasColumnType("datetime")
                    .HasComment("最后修改时间");

                entity.Property(e => e.Saleable)
                    .IsRequired()
                    .HasColumnName("saleable")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否上架，0下架，1上架");

                entity.Property(e => e.SubTitle)
                    .HasColumnName("sub_title")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasComment("子标题")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''")
                    .HasComment("标题")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Valid)
                    .IsRequired()
                    .HasColumnName("valid")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否有效，0已删除，1有效");
            });

            modelBuilder.Entity<TbSpuDetail>(entity =>
            {
                entity.HasKey(e => e.SpuId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_spu_detail");

                entity.Property(e => e.SpuId)
                    .HasColumnName("spu_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AfterService)
                    .HasColumnName("after_service")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("售后服务")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("商品描述信息")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.GenericSpec)
                    .IsRequired()
                    .HasColumnName("generic_spec")
                    .HasColumnType("varchar(2048)")
                    .HasDefaultValueSql("''")
                    .HasComment("通用规格参数数据")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PackingList)
                    .HasColumnName("packing_list")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("包装清单")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SpecialSpec)
                    .IsRequired()
                    .HasColumnName("special_spec")
                    .HasColumnType("varchar(1024)")
                    .HasComment("特有规格参数及可选值信息，json格式")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSku>(entity =>
            {
                entity.ToTable("tb_sku");

                entity.HasComment("sku表,该表表示具体的商品实体,如黑色的 64g的iphone 8");

                entity.HasIndex(e => e.SpuId)
                    .HasDatabaseName("key_spu_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("sku id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime")
                    .HasComment("添加时间");

                entity.Property(e => e.Enable)
                    .IsRequired()
                    .HasColumnName("enable")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否有效，0无效，1有效");

                entity.Property(e => e.Images)
                    .HasColumnName("images")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("商品的图片，多个图片以‘,’分割")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Indexes)
                    .HasColumnName("indexes")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''")
                    .HasComment("特有规格属性在spu属性模板中的对应下标组合")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnName("last_update_time")
                    .HasColumnType("datetime")
                    .HasComment("最后修改时间");

                entity.Property(e => e.OwnSpec)
                    .HasColumnName("own_spec")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("sku的特有规格参数键值对，json格式，反序列化时请使用linkedHashMap，保证有序")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("bigint(15)")
                    .HasComment("销售价格，单位为分");

                entity.Property(e => e.SpuId)
                    .HasColumnName("spu_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("spu id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(256)")
                    .HasComment("商品标题")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
