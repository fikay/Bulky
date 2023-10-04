using BulkyWeb.DataAccess.Repository;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            List<Product> productList= _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }

        public IActionResult Details(int  Id)
        {
            ShoppingCart cart = new ShoppingCart() 
            { Product = _unitOfWork.Product.Get(x => x.Id == Id, includeProperties: "Category"), 
              ProductId = Id,
              Count =1
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity =(ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            ShoppingCart DbCart = _unitOfWork.ShoppingCart.Get(u =>u.ProductId == cart.ProductId && u.ApplicationUserId == userId);

            if (DbCart != null)
            {
                DbCart.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(DbCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }

            
            _unitOfWork.save();
            TempData["success"] = "Cart Was updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
