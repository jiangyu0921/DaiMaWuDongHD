using DaiMaWuDong.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Spec.Model.Models
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
        public virtual DbSet<TbSpecGroup> TbSpecGroup { get; set; }
        public virtual DbSet<TbSpecParam> TbSpecParam { get; set; }

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

            modelBuilder.Entity<TbSpecGroup>(entity =>
            {
                entity.ToTable("tb_spec_group");

                entity.HasComment("规格参数的分组表，每个商品分类下有多个规格参数组");

                entity.HasIndex(e => e.Cid)
                    .HasDatabaseName("key_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("主键");

                entity.Property(e => e.Cid)
                    .HasColumnName("cid")
                    .HasColumnType("bigint(20)")
                    .HasComment("商品分类id，一个分类下有多个规格组");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(32)")
                    .HasComment("规格组的名称")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSpecParam>(entity =>
            {
                entity.ToTable("tb_spec_param");

                entity.HasComment("规格参数组下的参数名");

                entity.HasIndex(e => e.Cid)
                    .HasDatabaseName("key_category");

                entity.HasIndex(e => e.GroupId)
                    .HasDatabaseName("key_group");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .HasComment("主键");

                entity.Property(e => e.Cid)
                    .HasColumnName("cid")
                    .HasColumnType("bigint(20)")
                    .HasComment("商品分类id");

                entity.Property(e => e.Generic)
                    .HasColumnName("generic")
                    .HasComment("是否是sku通用属性，true或false");

                entity.Property(e => e.GroupId)
                    .HasColumnName("group_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(256)")
                    .HasComment("参数名")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Numeric)
                    .HasColumnName("numeric")
                    .HasComment("是否是数字类型参数，true或false");

                entity.Property(e => e.Searching)
                    .HasColumnName("searching")
                    .HasComment("是否用于搜索过滤，true或false");

                entity.Property(e => e.Segments)
                    .HasColumnName("segments")
                    .HasColumnType("varchar(1024)")
                    .HasDefaultValueSql("''")
                    .HasComment("数值类型参数，如果需要搜索，则添加分段间隔值，如CPU频率间隔：0.5-1.0")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''")
                    .HasComment("数字类型参数的单位，非数字类型可以为空")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
