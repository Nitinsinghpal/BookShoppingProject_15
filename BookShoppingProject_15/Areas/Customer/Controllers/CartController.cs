using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Models.ViewModels;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim==null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);

            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);

                if(list.Product.Description.Length>100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
                }
            }
            return View(ShoppingCartVM);
        }
        public IActionResult plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };
            ShoppingCartVM.OrderHeader.ApplicationUser= _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

            foreach(var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,list.Product.Price,list.Product.Price50,list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);

            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string StripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);

                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (orderDetails.Price * orderDetails.Count);
                _unitOfWork.OrderDetail.Add(orderDetails);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(SD.Ss_Session, 0);

            #region Stripe Payment

            if(StripeToken==null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "inr",
                    Description="OrderId :"+ShoppingCartVM.OrderHeader.Id,
                    Source=StripeToken
                };

                //Payment

                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if(charge.Status.ToLower()=="Succeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;

                }

            }
            #endregion
            _unitOfWork.Save();
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

    }
}
