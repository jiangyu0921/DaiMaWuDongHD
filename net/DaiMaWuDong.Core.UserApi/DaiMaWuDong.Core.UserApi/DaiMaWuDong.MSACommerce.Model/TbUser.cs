
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model
{
    //[SugarTable("tb_user")]
    public partial class TbUser//: BaseModel
    {
        public long? id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public DateTime? Created { get; set; }
        public string? Salt { get; set; }
    }
}
