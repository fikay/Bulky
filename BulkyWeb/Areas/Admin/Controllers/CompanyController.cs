using BulkyWeb.Models;
using BulkyWeb.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
       
        public CompanyController(IUnitOfWork unitOfWork)
        {
            // _unitOfWork = db;
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
           
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
           
            return View(CompanyList);
        }
        //Merging both Create and Update route
        public IActionResult Upsert(int ? id)
        {
            if(id == null || id ==0)
            {

                return View( new Company());
            }
            else
            {
                Company? companyFound = _unitOfWork.Company.Get(x => x.Id == id);
             
                return View(companyFound);
            }

            
           
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            
                if (ModelState.IsValid )
                {
                //string wwwRoot = _webHostEnvironment.WebRootPath;
                //if (file != null)
                //{
                //    //string[] r = Path.GetFileName(file.FileName).Split('.');
                   
                //    //string fileName = r[0] + Path.GetExtension(file.FileName);
                //    //string CompanyPath = Path.Combine(wwwRoot, @"images\Company");

                //    //    if(!String.IsNullOrEmpty(obj.Company.ImageUrl))
                //    //    {
                //    //        string oldPath = Path.Combine(wwwRoot, obj.Company.ImageUrl.Trim('\\'));

                //    //        if(System.IO.File.Exists(oldPath))
                //    //        {
                //    //            System.IO.File.Delete(oldPath);
                //    //        }
                //    //    }

                //    //    using (var fileStream = new FileStream(Path.Combine(CompanyPath, fileName), FileMode.Create))
                //    //    {
                //    //        file.CopyTo(fileStream);
                //    //    }

                //    //obj.Company.ImageUrl = @"\images\Company\" + fileName;

                //}

                    if(obj.Id != 0)
                    {
                        _unitOfWork.Company.Update(obj);
                        TempData["Success"] = "Company was updated successfully";
                    }
                    else
                    {
                        _unitOfWork.Company.Add(obj);
                        TempData["Success"] = "Company was created successfully";
                    }
                
                    _unitOfWork.save();
                    
                    return RedirectToAction("Index", "Company");
                }        
                else
                {
                    //obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    //{
                    //    Text = u.Name,
                    //    Value = u.Id.ToString()
                    //});
                    return View(obj);
                }
            
          
                //if (ModelState.IsValid)
                //{
                //    _unitOfWork.Company.Update(obj.Company);
                //    _unitOfWork.save();
                //    TempData["Success"] = "Category was updated successfully";
                //    return RedirectToAction("Index", "Company");
                //}
                //return View(); 


        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? categoryFound = _unitOfWork.Company.Get(x => x.Id == id);
        //    if (categoryFound == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(categoryFound);
        //}

        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(obj);
        //        _unitOfWork.save();
        //        TempData["Success"] = "Category was updated successfully";
        //        return RedirectToAction("Index", "Category");
        //    }
        //    return View();

        //}


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
           Company? companyFound = _unitOfWork.Company.Get(x => x.Id == id);
            if (companyFound == null)
            {
                return NotFound();
            }
            return View(companyFound);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
           Company? obj = _unitOfWork.Company.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Delete(obj);
            _unitOfWork.save();
            TempData["Success"] = "Company was deleted successfully";
            return RedirectToAction("Index", "Company");
        }
        //public bool checkDb(CompanyVM obj)
        //{

        //    if (_unitOfWork.Company.Get(c => c.Title == obj.Company.Title) != null || _unitOfWork.Company.Get(c => c.ISBN == obj.Company.ISBN) != null)
        //    {
        //        ModelState.AddModelError("", "The Company already Exists");
        //        return true;
        //    }
        //    return false;

        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { CompanyList });

        }
        [HttpDelete]
        public IActionResult deleteCompany(int id)
        {
            if ( id == 0)
            {
                return NotFound();
            }
            Company? CompanyFound = _unitOfWork.Company.Get(x => x.Id == id);
            if (CompanyFound == null)
            {
                return NotFound();
            }

            _unitOfWork.Company.Delete(CompanyFound);
            _unitOfWork.save();

            return Json(new {success =  true, message = "Delete Successful"});
        }
        #endregion

    }



}
