using BulkyWeb.Models;
using BulkyWeb.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            // _unitOfWork = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll().ToList();
            return View(ProductList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid && checkDb(obj) == false)
            {
                _unitOfWork.Product.Add(obj);
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
            Product? categoryFound = _unitOfWork.Product.Get(x => x.Id == id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
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
           Product? categoryFound = _unitOfWork.Product.Get(x => x.Id == id);
            if (categoryFound == null)
            {
                return NotFound();
            }
            return View(categoryFound);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
           Product? obj = _unitOfWork.Product.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(obj);
            _unitOfWork.save();
            TempData["Success"] = "Category was deleted successfully";
            return RedirectToAction("Index", "Category");
        }
        public bool checkDb(Product obj)
        {

            if (_unitOfWork.Product.Get(c => c.Title == obj.Title) != null || _unitOfWork.Product.Get(c => c.ISBN == obj.ISBN) != null)
            {
                ModelState.AddModelError("", "The Category or Display Order already Exists");
                return true;
            }
            return false;

        }
    }



}
