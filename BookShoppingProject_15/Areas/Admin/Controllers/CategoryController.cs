using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using BookShoppingProject_15.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    [Area("Admin")]
    public class CategoryController : Controller
    {       
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region GetAll
        public IActionResult GetAll()
        {
            var CategoryList = _unitOfWork.Category.GetAll();
            return Json(new { data = CategoryList });
        }
        #endregion


        #region Upsert
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
                return View(category);

            var CategoryInDb = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (CategoryInDb == null)
                return NotFound();
            return View(CategoryInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
                return NotFound();
            if (category.Id == 0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        #endregion


        #region Delete
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Error while deleting data" });
            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data deleted succesfully !!" });

        }
        #endregion
    }
}
