using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using BookShoppingProject_15.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mook.DapperCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]

    [Area("Admin")]
    public class CoverTypeController : Controller
    {

        //#region CoverType Entity
        ////private readonly IUnitOfWork _unitOfWork;
        //private readonly ApplicationDbContext _context;
        //public CoverTypeController(ApplicationDbContext context )
        //{
        //    _context = context;
        //    //_unitOfWork = unitOfWork;
        //}
        //#region Index
        //public IActionResult Index()
        //{
        //    return View();
        //}
        //#endregion

        //#region GetAll
        //public IActionResult GetAll()
        //{
        //    var CoverTypeInDb = _context.CoverTypes.ToList();
        //    return Json(new { data = CoverTypeInDb });
        //}
        //#endregion

        ////#region Delete
        ////[HttpDelete]
        ////public IActionResult Delete(int id)
        ////{
        ////    var CoverTypeInDb = _unitOfWork.CoverType.Get(id);
        ////    if (CoverTypeInDb == null)
        ////        return Json(new { success = false, message = "Error While Deleting Data" });
        ////    _unitOfWork.CoverType.Remove(CoverTypeInDb);
        ////    _unitOfWork.Save();
        ////    return Json(new { success = true, message = "Data Deleted Successfully" });

        ////}
        ////#endregion

        ////#region Upsert
        ////public IActionResult Upsert(int? id)
        ////{
        ////    CoverType coverType = new CoverType();
        ////    if (id == null)
        ////        return View(coverType);
        ////    coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
        ////    if (coverType == null)
        ////        return NotFound();

        ////    return View(coverType);
        ////}

        ////[HttpPost]
        ////[ValidateAntiForgeryToken]

        ////public IActionResult Upsert(CoverType coverType)
        ////{
        ////    if (coverType == null)
        ////        return NotFound();
        ////    if (!ModelState.IsValid)
        ////        return View(coverType);
        ////    if (coverType.Id == 0)
        ////        _unitOfWork.CoverType.Add(coverType);
        ////    else
        ////        _unitOfWork.CoverType.Update(coverType);
        ////    _unitOfWork.Save();
        ////    return RedirectToAction(nameof(Index));
        ////}
        ////#endregion

        //#endregion

        #region WithStoredProcedure

        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAll()
        {
            var CoverTypeList = _unitOfWork.SP_Call.List<CoverType>(SD.GetCoverTypes);
            return Json(new { data = CoverTypeList });

        }

        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@Id", id);
            _unitOfWork.SP_Call.Execute<CoverType>(SD.Proc_CoverType_Delete, param);
            return Json(new { success = true, message = "Data Deleted Successfully" });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(coverType);

            var param = new DynamicParameters();
            param.Add("@Name", coverType.Name);

            if (coverType.Id == 0)
                _unitOfWork.SP_Call.Execute<CoverType>(SD.Proc_CoverType_Create, param);
            else
            {
                param.Add("@Id", coverType.Id);
                _unitOfWork.SP_Call.Execute<CoverType>(SD.Proc_CoverType_Update, param);
            }
                
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
                return View(coverType);

            var param = new DynamicParameters();
            param.Add("@Id", id.GetValueOrDefault());

            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.GetCoverType, param);
            if (coverType == null)
                return NotFound();

            return View(coverType);
        }
        #endregion
    }
}
