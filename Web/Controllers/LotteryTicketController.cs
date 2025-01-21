using Common.Algorithm;
using Common;
using Microsoft.AspNetCore.Mvc;
using Model.LotteryTicket;
using System.Diagnostics;
using Web.Models;
using Web.Business.LotteryTicket;
using Common.Result;

namespace Web.Controllers
{
    public class LotteryTicketController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LotteryTicketController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PredicteLotteryTicket()
        {
            var result = new LotteryTicketService(_httpClientFactory).PredicteLotteryTicket();
            if (!result.IsSuccess)
            {
                return Json(result);
            }

            var message = string.Join("\n", result.Data.Select(o => string.Join(",", o)));
            return Json(Result<string>.WithSuccess(message));
        }
    }
}
