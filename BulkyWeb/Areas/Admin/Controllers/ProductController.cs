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
    [CustomAuthorize(SD.Role_Admin, SD.Role_Employee)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            // _unitOfWork = db;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {  
            return View();
        }
        //Merging both Create and Update route
        public IActionResult Upsert(int ? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }
           );  

            if(id == null || id ==0)
            {
                ProductVM ProductVM = new()
                {
                    CategoryList = CategoryList,
                    Product = new Product()
                };
                return View(ProductVM);
            }
            else
            {
                Product? categoryFound = _unitOfWork.Product.Get(x => x.Id == id, includeProperties:"ProductImages");
                ProductVM ProductVM = new()
                {
                    CategoryList = CategoryList,
                    Product = categoryFound
                };
                return View(ProductVM);
            }

            
           
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, List< IFormFile> ? files)
        {
            
                if (ModelState.IsValid )
                {


                        if (obj.Product.Id != 0)
                        {
                            _unitOfWork.Product.Update(obj.Product);
                            //TempData["Success"] = "Product was updated successfully";
                        }
                        else
                        {
                            _unitOfWork.Product.Add(obj.Product);
                            TempData["Success"] = "Product was created successfully";
                        }


                            string wwwRoot = _webHostEnvironment.WebRootPath;
                            if (files != null)
                            {

                                foreach (IFormFile file in files)
                                {
                                    string[] r = Path.GetFileName(file.FileName).Split('.');

                                    string fileName = r[0] + Path.GetExtension(file.FileName);
                                    string productPath = @"images\products\product-" + obj.Product.Id;
                                    string finalPath = Path.Combine(wwwRoot, productPath);

                                    if(!Directory.Exists(finalPath))
                                    {
                                        Directory.CreateDirectory(finalPath);
                                    }

                                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                                    {
                                        file.CopyTo(fileStream);
                                    }


                                    ProductImage productImages = new() { 
                                        productId = obj.Product.Id,
                                        ImageUrl = @"\"+productPath+@"\"+fileName,
                                    };

                                    if(obj.Product.ProductImages == null)
                                    {
                                        obj.Product.ProductImages = new List<ProductImage>();
                                    }

                                         obj.Product.ProductImages.Add(productImages);
                        
                                }
                            }
                _unitOfWork.Product.Update(obj.Product);
                             _unitOfWork.save();
                TempData["Success"] = "Product was created/updated successfully";
                return RedirectToAction("Index", "Product");
                 }        
                else
                {
                    obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    return View(obj);
                }
            
          
                //if (ModelState.IsValid)
                //{
                //    _unitOfWork.Product.Update(obj.Product);
                //    _unitOfWork.save();
                //    TempData["Success"] = "Category was updated successfully";
                //    return RedirectToAction("Index", "Product");
                //}
                //return View(); 


        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? categoryFound = _unitOfWork.Product.Get(x => x.Id == id);
        //    if (categoryFound == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(categoryFound);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
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
            TempData["Success"] = "Product was deleted successfully";
            return RedirectToAction("Index", "Product");
        }
        public bool checkDb(ProductVM obj)
        {

            if (_unitOfWork.Product.Get(c => c.Title == obj.Product.Title) != null || _unitOfWork.Product.Get(c => c.ISBN == obj.Product.ISBN) != null)
            {
                ModelState.AddModelError("", "The Product already Exists");
                return true;
            }
            return false;

        }

        public IActionResult DeleteImage(int imageId)
        {
            var ImagetoDelete = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            var productId = ImagetoDelete.productId;
            if(ImagetoDelete != null)
            {
                if(!string.IsNullOrEmpty(ImagetoDelete.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, ImagetoDelete.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Delete(ImagetoDelete);
                _unitOfWork.save();
                TempData["success"] = "Image Deleted Successfully";
            }

            return RedirectToAction(nameof(Upsert), new {id = productId  });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { ProductList });

        }
        [HttpDelete]
        public IActionResult deleteProduct(int id)
        {
            if ( id == 0)
            {
                return NotFound();
            }
            Product? productFound = _unitOfWork.Product.Get(x => x.Id == id);
            if (productFound == null)
            {
                return NotFound();
            }

            //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productFound.ImageUrl.TrimStart('\\'));
            //if(System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}

            _unitOfWork.Product.Delete(productFound);
            _unitOfWork.save();

            return Json(new {success =  true, message = "Delete Successful"});
        }
        #endregion
    }



}
