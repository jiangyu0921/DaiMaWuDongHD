using DaiMaWuDong.AgileFramework.Cache;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Service
{
    /// <summary>
    /// 用户登录后的购物车操作业务
    /// </summary>
    public class CartService : ICartService
    {
        private RedisClusterHelper _cacheClientDB;
        public CartService(RedisClusterHelper cacheClientDB)
        {
            _cacheClientDB = cacheClientDB;
        }
        private static readonly string KEY_PREFIX = "yt:cart:uid:";

        /// <summary>
        /// 添加购物车业务功能
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="loginUser"></param>
        public void AddCart(Cart cart, UserInfo loginUser)
        {
            //获取用户信息
            string key = KEY_PREFIX + loginUser.id;
            //获取商品ID
            string field = cart.skuId.ToString();
            //获取数量
            int num = cart.num;
            //获取hash操作的对象 【如果购物车中该商品存在直接数量+1】
            var hashOps = _cacheClientDB.ContainsKey(key, field).Result;
            if (hashOps)
            {
                cart = _cacheClientDB.HashGet<Cart>(key, field);
                cart.num = num + cart.num;
            }

            _cacheClientDB.HashSet(key, field, cart);
        }

        /// <summary>
        /// 用户登录时候批量把未登录前数据更新合并到redis中
        /// </summary>
        /// <param name="carts"></param>
        /// <param name="loginUser"></param>
        public void AddCarts(List<Cart> carts, UserInfo loginUser)
        {
            foreach (Cart cart in carts)
            {
                AddCart(cart, loginUser);
            }
        }

        /// <summary>
        /// 根据SkuId删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loginUser"></param>
        /// <exception cref="Exception"></exception>
        public void DeleteCart(long id, UserInfo loginUser)
        {
            string key = KEY_PREFIX + loginUser.id;
            var hashOps = _cacheClientDB.HashKeys(key);
            if (!hashOps.Contains(id.ToString()))
            {
                //该商品不存在
                throw new Exception("购物车商品不存在, 用户：" + loginUser.id + ", 商品：" + id);
            }
            //删除商品
            _cacheClientDB.HashRemoveForFields(key, id.ToString());
        }

        /// <summary>
        /// 根据选择的skuId集合批量删除购物车信息
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        public void DeleteCarts(List<long> ids, long userId)
        {
            string key = KEY_PREFIX + userId;
            var hashOps = _cacheClientDB.HashKeys(key);
            foreach (var item in ids)
            {
                if (hashOps.Contains(item.ToString()))
                {
                    //删除商品
                    _cacheClientDB.HashRemoveForFields(key, item.ToString());
                }
            }
        }

        /// <summary>
        /// 显示当前用户的购车信息
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public List<Cart> ListCart(UserInfo loginUser)
        {  //获取该用户Redis中的key
            string key = KEY_PREFIX + loginUser.id;
            if (_cacheClientDB.HashKeys(key) == null)
            { //Redis中没有给用户信息
                return null;
            }
            var hashOps = _cacheClientDB.HashValues(key);
            if (hashOps == null || hashOps.Count <= 0)
            {
                //购物车中无数据
            }
            List<Cart> carts = new List<Cart>();
            foreach (var item in hashOps)
            {
                carts.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Cart>(item));
            }
            return carts;
        }

        /// <summary>
        /// 更新购物车中商品的数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        /// <param name="loginUser"></param>
        /// <exception cref="Exception"></exception>
        public void UpdateNum(long id, int num, UserInfo loginUser)
        {
            //获取该用户Redis中的key
            string key = KEY_PREFIX + loginUser.id;
            var hashOps = _cacheClientDB.HashKeys(key);
            if (!hashOps.Contains(id.ToString()))
            {
                //该商品不存在
                throw new Exception("购物车商品不存在, 用户：" + loginUser.id + ", 商品：" + id);
            }
            //查询购物车商品
            var cart = _cacheClientDB.HashGet<Cart>(key, id.ToString());
            //修改数量
            cart.num = num;
            // 回写Redis
            _cacheClientDB.HashSet(key, id.ToString(), cart);
        }
    }
}
