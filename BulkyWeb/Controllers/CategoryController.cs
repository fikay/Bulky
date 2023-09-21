using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository db)
        {
            categoryRepository = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = categoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(ModelState.IsValid  && checkDb(obj) == false)
            {
                categoryRepository.Add(obj);
                categoryRepository.Save();
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
             Category? categoryFound = categoryRepository.Get(x=>x.Id==id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid )
            {
               categoryRepository.Update(obj);
                categoryRepository.Save();
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
            Category? categoryFound = categoryRepository.Get(x => x.Id == id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category ? obj = categoryRepository.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            categoryRepository.Delete(obj);
            categoryRepository.Save();
            TempData["Success"] = "Category was deleted successfully";
            return RedirectToAction("Index", "Category");
        }
        public bool checkDb(Category obj)
        {

            if (categoryRepository.Get(c=>c.Name == obj.Name) != null || categoryRepository.Get(c => c.DisplayOrder == obj.DisplayOrder) != null) 
            {
                ModelState.AddModelError("", "The Category or Display Order already Exists");
                return true;
            }
           return false;
              
        }
    }


    
}
