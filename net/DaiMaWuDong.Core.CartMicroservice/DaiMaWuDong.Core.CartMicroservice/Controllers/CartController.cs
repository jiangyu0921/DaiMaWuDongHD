
using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DaiMaWuDong.Core.CartMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        /**
		 * 添加商品到购物车
		 *
		 * @param cart
		 * @return
		 */
        [Route("/api/cart/add")]
        [Authorize]
        [HttpPost]
        public JsonResult AddCart([FromBody] Cart cart)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            _cartService.AddCart(cart, user);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "添加成功"
            });
        }

        /**
         * 批量添加商品到购物车
         *
         */
        [Authorize]
        [Route("/api/cart/batch")]
        [HttpPost]
        public JsonResult AddCartBatch([FromBody] List<Cart> carts)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            _cartService.AddCarts(carts, user);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "添加成功"
            });
        }


        /**
         * 从购物车中删除商品
         *
         * @param id
         * @return
         */
        [Authorize]
        [Route("/api/cart/{id}")]
        [HttpDelete]
        public JsonResult DeleteCart(long id)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            _cartService.DeleteCart(id, user);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "删除成功"
            });
        }



        /**
		 * 更新购物车中商品的数量
		 *
		 * @param id  商品ID
		 * @param num 修改后的商品数量
		 * @return
		 */

        [HttpPut]
        [Authorize]
        [Route("/api/cart/update")]
        public JsonResult UpdateNum([FromForm] long id, [FromForm] int num)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            _cartService.UpdateNum(id, num, user);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "更新成功"
            });
        }


        /**
		 * 查询购物车
		 *
		 * @return
		 */
        [Route("/api/cart/list")]
        [HttpGet]
        [Authorize]
        public JsonResult ListCart()
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();
            var cartList = _cartService.ListCart(user);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "获取成功",
                Value = cartList
            });

        }
    }
}
