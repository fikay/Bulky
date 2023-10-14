using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<IdentityUser> _userManager;
        
       
        ApplicationDbContext _db;

        

        public UserController( IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
          
            return View();
        }

        public IActionResult UserPermission( string id)
        {
            RoleManagementVM rolemanager = new RoleManagementVM()
            {
                User = _unitOfWork.ApplicationUser.Get(x => x.Id == id),
                Roles = _db.Roles.Select(u => new SelectListItem
                {
                    Value = u.Name,
                    Text = u.Name

                }).ToList(),
                CompanyList = _db.Companies.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
           var role = _db.Roles.Where(x => x.Id == _db.UserRoles.FirstOrDefault(x => x.UserId == id).RoleId).ToList();
            if(role.Count > 0) {
               rolemanager.User.Role = role[0].Name;
            }
            return View(rolemanager);
        }

        [HttpPost, ActionName("UserPermission")]
        
        public IActionResult UpdateUserPermission(RoleManagementVM roleManagement)
        {
           var userRole =  _db.UserRoles.AsNoTracking().FirstOrDefault(x => x.UserId == roleManagement.User.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == userRole).Name;

            
                if (!(roleManagement.User.Role == oldRole))
                {
               
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(x => x.Id == roleManagement.User.Id);
                
                    if (roleManagement.User.Role == SD.Role_Company)
                    {
                        applicationUser.CompanyId = roleManagement.User.CompanyId;
                    }
                    if (oldRole == SD.Role_Company)
                    {
                        applicationUser.CompanyId = null;
                    }
                    _db.SaveChanges();
                   

                    _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(applicationUser, roleManagement.User.Role).GetAwaiter().GetResult();

                }
           
           
            return RedirectToAction(nameof(Index));
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company").ToList();
            foreach (var user in userList)
            {
              var x= _db.Roles.Where(u => u.Id == _db.UserRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId).ToList();
                if(x.Count > 0)
                {
                    user.Role = x[0].Name;
                }
             
            }
            return Json(new { userList });

        }

        //[HttpGet]
        //public IActionResult UserPermission([FromBody] string id)
        //{



        //    return Json(new { success = true, message = "Permissions Updated" });
        //}


        //Unlock or Lock users
        [HttpPost]
        public IActionResult Lock([FromBody]string id)
        {
            string message = null;
            var objDb = _db.ApplicationUsers.FirstOrDefault(U => U.Id == id);
            if (objDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }

            if(objDb.LockoutEnd != null && objDb.LockoutEnd>DateTime.Now)
            {
                //locked user needs to be unlocked
                objDb.LockoutEnd = DateTime.Now;
                message = "User Unlocked";
            }
            else
            {
                objDb.LockoutEnd = DateTime.Now.AddYears(1000);
                message = "User locked";
            }

            _db.SaveChanges();

            return Json(new { success = true, message = message });
        }
        #endregion
    }




}
