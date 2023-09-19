using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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
             Category categoryFound = _db.Categories.Find(id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();


        }
        public bool checkDb(Category obj)
        {

            if(_db.Categories.Where(c=>c.Name == obj.Name).FirstOrDefault() != null || _db.Categories.Where(c => c.DisplayOrder == obj.DisplayOrder).FirstOrDefault() != null) 
            {
                ModelState.AddModelError("", "The Category or Display Order already Exists");
                return true;
            }
           return false;
              
        }
    }


    
}
