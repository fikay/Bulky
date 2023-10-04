using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer") ]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
                
            };

            foreach (ShoppingCart cart in ShoppingCartVM.ShoppingCartList )
            {
                cart.Price = GetPriceBasedOnQuantity( cart );
                ShoppingCartVM.OrderTotal += (cart.Price* cart.Count); 
            }
           
            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(x=>x.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            if(cartFromDb.Count <= 1)
            {
                delete(cartId);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.save();
                
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            if(cartFromDb != null)
            {
                _unitOfWork.ShoppingCart.Delete(cartFromDb);
                _unitOfWork.save();
            }
           
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if ( shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
