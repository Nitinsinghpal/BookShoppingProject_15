﻿using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using BookShoppingProject_15.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_15.Areas.Admin.Controllers
{
    //[Authorize(Roles =SD.Role_Admin + SD.Role_Employee)]

    [Area("Admin")]
    public class UserController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.Include(c => c.Company).ToList();
            var roles = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;

                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if(user.Company==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            if(!User.IsInRole(SD.Role_Admin))
            {
                var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
                userList.Remove(adminUser);
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            bool isLocked = false;
            var UserInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (UserInDb == null)
                return Json(new { success = false, message = "Error While locking And Unlocking data" });
            if(UserInDb!=null && UserInDb.LockoutEnd>DateTime.Now)
            {
                UserInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                UserInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User successfully Locked" : "User Successfully UnLocked" });
        }
    }
}
