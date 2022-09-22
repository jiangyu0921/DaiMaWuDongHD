using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Model.Models
{
    public partial class TbSpuDetail
    {
        public long SpuId { get; set; }
        public string Description { get; set; }
        public string GenericSpec { get; set; }
        public string SpecialSpec { get; set; }
        public string PackingList { get; set; }
        public string AfterService { get; set; }
    }
}
