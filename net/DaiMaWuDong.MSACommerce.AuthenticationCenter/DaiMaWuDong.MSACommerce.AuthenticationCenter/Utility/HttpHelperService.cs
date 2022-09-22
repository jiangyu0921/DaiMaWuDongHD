using DaiMaWuDong.Common.Model;

namespace DaiMaWuDong.MSACommerce.AuthenticationCenter.Utility
{
    public class HttpHelperService
    {
        /// <summary>
        /// 调用校验服务
        /// </summary>
        /// <param name="userUrl"></param>
        /// <returns></returns>
        public AjaxResult<T> VerifyUser<T>(string userUrl)
        {
            AjaxResult<T> ajaxResult = null;
            HttpResponseMessage sResult = this.HttpRequest(userUrl, HttpMethod.Get, null);
            if (sResult.IsSuccessStatusCode)
            {
                string content = sResult.Content.ReadAsStringAsync().Result;
                ajaxResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AjaxResult<T>>(content);
            }
            else
            {
                ajaxResult = new AjaxResult<T>()
                {
                    StatusCode = (int)sResult.StatusCode,
                    Result = false,
                };
            }
            return ajaxResult;
        }

        public HttpResponseMessage HttpRequest(string url, HttpMethod httpMethod, Dictionary<string, string> parameter)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage()
                {
                    Method = httpMethod,
                    RequestUri = new Uri(url)
                };
                if (parameter != null)
                {
                    var encodedContent = new FormUrlEncodedContent(parameter);
                    message.Content = encodedContent;
                }
                return httpClient.SendAsync(message).Result;
            }
        }


    }
}
