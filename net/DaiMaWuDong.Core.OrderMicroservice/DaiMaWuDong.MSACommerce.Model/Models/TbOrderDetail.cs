using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model.Models
{
    public partial class TbOrderDetail
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long SkuId { get; set; }
        public int Num { get; set; }
        public string Title { get; set; }
        public string OwnSpec { get; set; }
        public long Price { get; set; }
        public string Image { get; set; }
    }
}
