using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Detail.Model.Models
{
    public partial class TbSpecGroup
    {
        public long Id { get; set; }
        public long Cid { get; set; }
        public string Name { get; set; }

        public List<TbSpecParam> Params = new List<TbSpecParam>();
    }
}
