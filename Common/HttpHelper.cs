using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HttpHelper
    {
        /// <summary>
        /// Post请求，五秒超时
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Get(IHttpClientFactory httpClientFactory, string url)
        {
            var client = httpClientFactory.CreateClient("httpClient");
            client.Timeout = TimeSpan.FromSeconds(5);//超时时间设定为五秒

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await client.SendAsync(request);
        }

        /// <summary>
        /// Post请求，五秒超时
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post(IHttpClientFactory httpClientFactory, string url, Dictionary<string, object> postData)
        {
            var client = httpClientFactory.CreateClient("httpClient");
            client.Timeout = TimeSpan.FromSeconds(5);//超时时间设定为五秒

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json")
            };
            return await client.SendAsync(request);
        }
    }
}
