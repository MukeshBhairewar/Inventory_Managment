using Inventory_Managment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inventory_Managment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventoryManagementContext InventoryManagementContext;


        public HomeController(ILogger<HomeController> logger,InventoryManagementContext InventoryManagementContext)
        {
            _logger = logger;
            this.InventoryManagementContext = InventoryManagementContext;
        }

        public IActionResult Index()
        { 

            var ItemMaster = InventoryManagementContext.ItemMasters.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Services()
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