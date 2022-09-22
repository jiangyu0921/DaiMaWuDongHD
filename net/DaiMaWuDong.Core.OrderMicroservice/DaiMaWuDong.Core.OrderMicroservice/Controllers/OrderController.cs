
using DaiMaWuDong.AgileFramework.Common;
using DaiMaWuDong.MSACommerce.DTOModel;
using DaiMaWuDong.MSACommerce.Interface;
using DaiMaWuDong.MSACommerce.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DaiMaWuDong.AgileFramework.FilterAttribute;
using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Model.Models;
using DaiMaWuDong.MSACommerce.DTOModel.DTO;

namespace DaiMaWuDong.Core.OrderMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        /**
		  * 创建订单
		  *
		  * @param orderDto
		  * @return
		  */
        [HttpPost]
        [Route("/api/order/create")]
        [TypeFilter(typeof(CustomAction2CommitFilterAttribute))]//避免重复提交
        public JsonResult CreateOrder([FromBody] OrderDto orderDto)
        {
            UserInfo user = base.HttpContext.GetCurrentUserInfo();// sign 签名保证数据不会被篡改
            var orderId = this._orderService.CreateOrder(orderDto, user);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "下单成功",
                Value = new
                {
                    status = 300,
                    orderId = orderId
                }
            });
        }


        /**
		 * 根据订单ID查询订单详情
		 *
		 * @param orderId
		 * @return
		 */
        [Route("/api/order/{id}")]
        [HttpGet]
        public JsonResult QueryOrderById(long id)
        {
            var order = _orderService.QueryById(id);

            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "查询成功",
                Value = JsonConvert.SerializeObject(order)
            });
        }



        /**
		 * 分页查询所有订单
		 *
		 * @param page
		 * @param rows
		 * @return
		 */
        [Route("/api/order/list")]
        [HttpGet]
        public JsonResult QueryOrderByPage(int page, int rows)
        {
            PageResult<TbOrder> result = _orderService.QueryOrderByPage(page, rows);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "查询成功",
                Value = result
            });
        }
    }
}
