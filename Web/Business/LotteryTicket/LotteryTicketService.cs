using Common;
using Common.Algorithm;
using Common.Result;
using Model.LotteryTicket;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using Web.Models;

namespace Web.Business.LotteryTicket
{
    public class LotteryTicketService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LotteryTicketService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 探数彩票接口Url
        /// </summary>
        private readonly string TanShuLotteryTicketUrl = "https://api.tanshuapi.com/api/caipiao/v1/query?key=24f6d9a2ecf990685691a5506cb26789&caipiaoid=14";

        /// <summary>
        /// 调用探数彩票接口获取数据
        /// </summary>
        /// <param name="issueno">期号，不传查询最新一期</param>
        /// <returns></returns>
        private Result<string> GetTanShuLotteryTicket(int? issueno = null)
        {
            try
            {
                var url = TanShuLotteryTicketUrl;
                if (issueno.HasValue)
                {
                    url += $"&issueno={issueno}";
                }

                var response = HttpHelper.Get(_httpClientFactory, url).Result;
                response.EnsureSuccessStatusCode();// 如果状态码不是200-299之间，则抛出HttpRequestException

                return Result<string>.WithSuccess(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return Result<string>.WithError(ex);
            }
        }

        /// <summary>
        /// 预测下期彩票
        /// 1、三次指数平滑算法
        /// 2、直线回归算法
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Result<List<List<decimal>>> PredicteLotteryTicket()
        {
            try
            {
                //获取当期彩票
                var lotteryTicketResult = GetTanShuLotteryTicket();
                if (!lotteryTicketResult.IsSuccess)
                {
                    return Result<List<List<decimal>>>.WithError(lotteryTicketResult.Exception);
                }

                var data = lotteryTicketResult.Data;
                if (string.IsNullOrEmpty(data))
                {
                    return Result<List<List<decimal>>>.WithError("未加载到当期彩票");
                }

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<TanShuCaiPiao14Model>(data);
                var issueno = model?.Data?.Issueno ?? 0;
                if (issueno <= 0)
                {
                    return Result<List<List<decimal>>>.WithError("未加载到当期彩票");
                }

                //获取前6期彩票
                var arrays = new List<List<decimal>>();
                for (var i = 5; i >= 0; i--)
                {
                    var tempLotteryTicketResult = GetTanShuLotteryTicket(issueno - i);
                    if (!tempLotteryTicketResult.IsSuccess)
                    {
                        return Result<List<List<decimal>>>.WithError(lotteryTicketResult.Exception);
                    }

                    var tempData = tempLotteryTicketResult.Data;
                    if (string.IsNullOrEmpty(tempData))
                    {
                        return Result<List<List<decimal>>>.WithError($"未加载到{issueno - i}期彩票");
                    }

                    var tempModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TanShuCaiPiao14Model>(tempData);
                    var tempIssueno = tempModel?.Data?.Issueno ?? 0;
                    if (tempIssueno <= 0)
                    {
                        return Result<List<List<decimal>>>.WithError($"未加载到{issueno - i}期彩票");
                    }

                    var first5 = tempModel?.Data?.Number?.Split(" ") ?? new string[0];
                    var last2 = tempModel?.Data?.Refernumber?.Split(" ") ?? new string[0];
                    var array = first5.Concat(last2).Select(o => Convert.ToDecimal(o)).ToList();
                    arrays.Add(array);
                }

                //预测
                var result1 = new List<decimal>();
                var result2 = new List<decimal>();
                for (var i = 0; i < 7; i++)
                {
                    var array = arrays.Select(o => o[i]).ToArray();
                    result1.Add(Math.Round(TripleExponentialSmoothingAlgorithm.CalcAccuratePredictiveValue(array), 2));
                    result2.Add(Math.Round(LinearRegressionAlgorithm.CalcPredictiveValue(array), 2));
                }

                return Result<List<List<decimal>>>.WithSuccess(new List<List<decimal>>() { result1, result2 });
            }
            catch (Exception ex)
            {
                return Result<List<List<decimal>>>.WithError(ex);
            }
        }
    }
}
