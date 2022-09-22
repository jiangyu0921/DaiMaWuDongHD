using DaiMaWuDong.MSACommerce.Detail.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DaiMaWuDong.MSACommerce.PageDetail.Controllers
{
    public class PageDetaiController : Controller
    {
        private IPageDetailService _pageDetailService;
        public PageDetaiController(IPageDetailService pageDetailService)
        {
            _pageDetailService = pageDetailService;
        }

        [Route("/item/{id}.html")]
        public IActionResult Index(long id)
        {
            var htmlmodel = _pageDetailService.loadModel(id);

            return View(htmlmodel);
        }
    }
}
