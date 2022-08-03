using BookShoppingProject.DataAccess.Repository;
using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using BookShoppingProject_15.Models;
using BookShoppingProject_15.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(productList);
           
        }

        public IActionResult Details(int id)
        {
            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");
            if (productInDb == null)
                return NotFound();
            var ShoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            return View(ShoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCartobj)
        {
            shoppingCartobj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                shoppingCartobj.ApplicationUserId = claim.Value;
                var ShoppinCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCartobj.ProductId);

                if (ShoppinCartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCartobj);
                }
                else
                {
                    ShoppinCartFromDb.Count += shoppingCartobj.Count;
                }
                _unitOfWork.Save();

                //session

                //var claimIdentitiy = (ClaimsIdentity)User.Identity;
                //var claim = claimIdentitiy.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                    HttpContext.Session.SetInt32(SD.Ss_Session, count);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCartobj.ProductId, includeProperties: "Category,CoverType");
                var ShoppingCart = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(ShoppingCart);
            }
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



//var productList = _unitOfWork.Product.GetAll(includeProperties: "Category, CoverType");
//return View(productList);