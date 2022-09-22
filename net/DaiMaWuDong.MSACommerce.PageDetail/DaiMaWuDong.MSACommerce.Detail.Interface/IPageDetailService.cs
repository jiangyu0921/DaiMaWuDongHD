using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Detail.Interface
{
    public interface IPageDetailService
    {
        public Dictionary<string, object> loadModel(long spuId);
    }
}
