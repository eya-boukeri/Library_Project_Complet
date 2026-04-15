using Library.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        public HomeController(IBookService bookService) => _bookService = bookService;

        public async Task<IActionResult> Index()
        {
            var dashboard = await _bookService.GetDashboardDataAsync();
            return View(dashboard);
        }

        public IActionResult Error() => View();
    }
}
