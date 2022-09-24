using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiceDataAccess.Repository.IRepository;
using SpicesApp.Models;
using SpicesModels.Models;
using SpicesModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpicesApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMenuItemRepository _MenuItemRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly ICouponRepository _couponcatRepo;

        public HomeController(ILogger<HomeController> logger, IMenuItemRepository MenuItemRepo
            , ICategoryRepository catRepo, ICouponRepository couponcatRepo)
        {
            _logger = logger;
            _MenuItemRepo = MenuItemRepo;
            _catRepo = catRepo;
            _couponcatRepo = couponcatRepo;
        }


        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                Category= _catRepo.GetAll(),
                MenuItem= _MenuItemRepo.GetAll(includeProperties: "Category,SubCategory"),
                Coupon=_couponcatRepo.GetAll(filter:x=>x.IsActive)

            };
            return View(model);
        }


        [Authorize]
        public  IActionResult Details(int id)
        {
            var menuItemFromDb = _MenuItemRepo.FirstOrDefault(filter: x => x.Id == id, includeProperties: "Category,SubCategory");

            ShoppingCart cartObj = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(cartObj);
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
