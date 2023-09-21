using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            // _unitOfWork = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid && checkDb(obj) == false)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.save();
                TempData["Success"] = "Category was created successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();


        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFound = _unitOfWork.Category.Get(x => x.Id == id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.save();
                TempData["Success"] = "Category was updated successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFound = _unitOfWork.Category.Get(x => x.Id == id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(obj);
            _unitOfWork.save();
            TempData["Success"] = "Category was deleted successfully";
            return RedirectToAction("Index", "Category");
        }
        public bool checkDb(Category obj)
        {

            if (_unitOfWork.Category.Get(c => c.Name == obj.Name) != null || _unitOfWork.Category.Get(c => c.DisplayOrder == obj.DisplayOrder) != null)
            {
                ModelState.AddModelError("", "The Category or Display Order already Exists");
                return true;
            }
            return false;

        }
    }



}
