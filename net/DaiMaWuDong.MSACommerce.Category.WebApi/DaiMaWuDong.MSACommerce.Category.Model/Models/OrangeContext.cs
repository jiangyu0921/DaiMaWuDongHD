using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Category.Model.Models
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

        public virtual DbSet<TbCategory> TbCategory { get; set; }
        public virtual DbSet<TbCategoryBrand> TbCategoryBrand { get; set; }

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

            modelBuilder.Entity<TbCategory>(entity =>
            {
                entity.ToTable("tb_category");

                entity.HasComment("商品类目表，类目和商品(spu)是一对多关系，类目与品牌是多对多关系");

                entity.HasIndex(e => e.ParentId)
                    .HasDatabaseName("key_parent_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("类目id");

                entity.Property(e => e.IsParent)
                    .HasColumnName("is_parent")
                    .HasComment("是否为父节点，0为否，1为是");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(32)")
                    .HasComment("类目名称")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("父类目id,顶级类目填0");

                entity.Property(e => e.Sort)
                    .HasColumnName("sort")
                    .HasColumnType("int(4)")
                    .HasComment("排序指数，越小越靠前");
            });

            modelBuilder.Entity<TbCategoryBrand>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.BrandId })
                    .HasName("PRIMARY");

                entity.ToTable("tb_category_brand");

                entity.HasComment("商品分类和品牌的中间表，两者是多对多关系");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("商品类目id");

                entity.Property(e => e.BrandId)
                    .HasColumnName("brand_id")
                    .HasColumnType("bigint(20)")
                    .HasComment("品牌id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
