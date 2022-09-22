using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Model.Models
{
    public partial class TbSeckillSku
    {
        public long Id { get; set; }
        public long SkuId { get; set; }
        public string Title { get; set; }
        public double SeckillPrice { get; set; }
        public string Image { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Enable { get; set; }

        [NotMapped]
        public DateTime CurrentTime { get; set; }

        [NotMapped]
        public int? Stock { get; set; }
        [NotMapped]
        public double Price { get; set; }
        [NotMapped]
        public int? SeckillTotal { get; set; }
    }
}
