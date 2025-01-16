using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class LotteryTicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
