using Kaiweb.Models;
using KaiWeb.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductsRepository _ProductsRepo;

        public HomeController(ILogger<HomeController> logger, IProductsRepository productsRepo)
        {
            _logger = logger;
            _ProductsRepo = productsRepo;
        }

        public IActionResult Index()
        {   
            IEnumerable<Products> productList = _ProductsRepo.GetAll(includeProperties:"myProperty");
            // 這邊會去Views資料夾中找有沒有叫 Index 的檔案
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            Products product = _ProductsRepo.Get(u=>u.Id == id, includeProperties:"myProperty");
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
