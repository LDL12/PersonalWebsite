using Common.Algorithm;
using Common;
using Microsoft.AspNetCore.Mvc;
using Model.LotteryTicket;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class LotteryTicketController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LotteryTicketController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> GetTanShuCaiPiao(int? issueno = null)
        {
            string? result;
            try
            {
                var url = "https://api.tanshuapi.com/api/caipiao/v1/query?key=24f6d9a2ecf990685691a5506cb26789&caipiaoid=14";
                if (issueno.HasValue)
                {
                    url += $"&issueno={issueno}";
                }

                var response = await HttpHelper.Get(_httpClientFactory, url);
                response.EnsureSuccessStatusCode();// 如果状态码不是200-299之间，则抛出HttpRequestException
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public async Task<IActionResult> Index()
        {
            //获取当期彩票
            var data = await GetTanShuCaiPiao();
            if (string.IsNullOrEmpty(data))
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            //当期彩票数据解析
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<TanShuCaiPiao14Model>(data);
            var issueno = model?.Data?.Issueno ?? 0;
            if (issueno <= 0)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            //获取前6期彩票
            var arrays = new List<decimal[]>();
            for (var i = 5; i >= 0; i--)
            {
                var temp_data = await GetTanShuCaiPiao(issueno - i);
                if (string.IsNullOrEmpty(temp_data))
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                var temp_model = Newtonsoft.Json.JsonConvert.DeserializeObject<TanShuCaiPiao14Model>(temp_data);
                var temp_issueno = temp_model?.Data?.Issueno ?? 0;
                if (temp_issueno > 0)
                {
                    var first5 = temp_model?.Data?.Number?.Split(" ") ?? new string[0];
                    var last2 = temp_model?.Data?.Refernumber?.Split(" ") ?? new string[0];
                    var array = first5.Concat(last2).Select(o => Convert.ToDecimal(o)).ToArray();
                    arrays.Add(array);
                }
            }

            //预测
            var result1 = new List<decimal>();
            var result2 = new List<decimal>();
            for (var i = 0; i < 7; i++)
            {
                var array = arrays.Select(o => o[i]).ToArray();
                result1.Add(TripleExponentialSmoothingAlgorithm.CalcAccuratePredictiveValue(array));
                result2.Add(LinearRegressionAlgorithm.CalcPredictiveValue(array));
            }


            return View();
        }
    }
}
