using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Seckill.Model;
using DaiMaWuDong.MSACommerce.Seckill.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Seckill.Interface
{
    public interface ISeckillService
    {
        long? CheckSeckillOrder(long goodsId, long v);
        bool CheckSeckillPath(long skuId, long id, string path);
        bool CheckVerifyCode(UserInfo userInfo, long goodsId, string verifyCode);
        string CreatePath(long goodsId, long id);
        byte[] CreateVerifyCode(UserInfo userInfo, string v);
        SeckillGoods QueryGoodsInfoFormCache(long skuId);
        List<SeckillGoods> QuerySecKillList();
        void SendMessage(SeckillDTO dto);
    }
}
