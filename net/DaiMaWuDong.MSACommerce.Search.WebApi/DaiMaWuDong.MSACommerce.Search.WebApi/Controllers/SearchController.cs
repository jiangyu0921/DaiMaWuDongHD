using DaiMaWuDong.Common.Model;
using DaiMaWuDong.MSACommerce.Search.Interface;
using DaiMaWuDong.MSACommerce.Search.Model.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaiMaWuDong.MSACommerce.Search.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [Route("page")]
        [HttpPost]
        public JsonResult Search([FromBody] SearchRequest searchRequest)
        {
            SearchResult<Goods> searchResult = _searchService.GetData(searchRequest);

            AjaxResult ajaxResult = new AjaxResult()
            {
                StatusCode = 200,
                Message = "success",
                Value = searchResult,
                Result = true
            };
            return new JsonResult(ajaxResult);
        }
        //public string Search([FromBody] SearchRequest searchRequest)
        //{
        //    SearchResult<Goods> searchResult = this._searchService.GetData(searchRequest);

        //    string sResult = JsonConvert.SerializeObject(new AjaxResult<SearchResult<Goods>>()
        //    {
        //        StatusCode = 200,
        //        Message = "success",
        //        Value = searchResult,
        //        TValue = searchResult,
        //        Result = true
        //    });
        //    HttpContext.Response.ContentType = "application/json; charset=utf-8";

        //    return sResult;
        //    //return new JsonResult();
        //}



        [Route("ImpData")]
        [HttpGet]
        public void ImpData()
        {
            _searchService.ImpDataBySpu();
        }


    }
}
