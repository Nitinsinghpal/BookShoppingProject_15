using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Models.ViewModels;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]

    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region GetAll
        public IActionResult GetAll()
        {
            var ProductList = _unitOfWork.Product.GetAll();
            return Json(new { data = ProductList });
        }

        #endregion

        #region Upsert
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(cl=>new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ct=>new SelectListItem()
                {
                    Text = ct.Name,
                    Value=ct.Id.ToString()
                })
            };
            if (id == null)
                return View (productVM);
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            return View(productVM);      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Upsert(ProductVM productVM)
        {
            #region Commented ModelState 
            //if(ModelState.IsValid)
            //{
            //    var webRootPath = _webHostEnvironment.WebRootPath;
            //    var files = HttpContext.Request.Form.Files;
            //    if(files.Count>0)
            //    {
            //        var fileName = Guid.NewGuid().ToString();
            //        var uploads = Path.Combine(webRootPath, @"images\products");
            //        var extension = Path.GetExtension(files[0].FileName);
            //    }
            //    if(productVM.Product.Id!=0)
            //    {
            //        var imageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
            //        productVM.Product.ImageUrl = imageExists;
            //    }
            //    if(productVM.Product.ImageUrl!=null)
            //    {
            //        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
            //        if(System.IO.File.Exists(imagePath))
            //        {
            //            System.IO.File.Delete(imagePath);
            //        }

            //    }
            //    //using (var fileStream = new FileStream(Path.Combine(uploads + fileName + extension), FileMode.Create))
            //    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            //    {
            //        files[0].CopyTo(fileStream);
            //    }

            //}
            #endregion
            if (ModelState.IsValid)
            {
                var WebRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(WebRootPath, @"Images\Products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (productVM.Product.Id != 0)
                    {
                        var ImageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = ImageExists;
                    }
                    if (productVM.Product.ImageUrl != null)
                    {
                        var Imagepath = Path.Combine(WebRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(Imagepath))
                        {
                            System.IO.File.Delete(Imagepath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\Images\Products\" + fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                }

                if (productVM.Product.Id == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()
                {
                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    })
                };
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }
        #endregion

        #region Delete

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitOfWork.Product.Get(id);
            if (productInDb == null)
                return Json(new { success = false, message = "Error while delete data" });
            if(productInDb.ImageUrl != "")
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, productInDb.ImageUrl.Trim('\\'));

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data deleted successfully" });
        }

        #endregion
    }
}
