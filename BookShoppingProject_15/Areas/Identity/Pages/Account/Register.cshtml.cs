﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BookShoppingProject_15.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,RoleManager<IdentityRole> roleManager,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Required]
            public String Name { get; set; }
            [Display(Name ="Address")]
            public String StreetAddress { get; set; }
            public String City { get; set; }
            public String State { get; set; }
            [Display(Name ="Postal code")]
            public String PostalCode { get; set; }
            [Display(Name = "Phone Number")]
            public String PhoneNumber { get; set; }
            public int? CompanyId { get; set; }
            public String Role { get; set; }

            //Company And Roles
            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }

           


        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_Individual).Select(x => x.Name).Select(rl => new SelectListItem()
                {
                    Text = rl,
                    Value = rl
                })

            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var user = new ApplicationUser()
                {
                    Name = Input.Name,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    State = Input.State,
                    PostalCode = Input.PostalCode,
                    CompanyId = Input.CompanyId,
                    Role = Input.Role,             
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if(!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if(!await _roleManager.RoleExistsAsync(SD.Role_Individual))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Individual));
                    }
                    if(! await _roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    if(!await _roleManager.RoleExistsAsync(SD.Role_Company))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
                    }


                    //Admin Role
                    //await _userManager.AddToRoleAsync(user, SD.Role_Admin);

                    if(user.Role == null && user.CompanyId==null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Individual);
                    }
                    else
                    {
                        if(user.CompanyId>0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_Company);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_Employee);
                        }
                    }


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if(user.Role==SD.Role_Individual || user.Role==null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                       
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
