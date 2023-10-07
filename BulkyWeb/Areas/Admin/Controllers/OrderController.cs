using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModels;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [CustomAuthorizeAttribute(SD.Role_Admin, SD.Role_Employee)]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;


		public OrderController( IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
        
        public IActionResult Index()
		{

            string status = HttpContext.Request.Query["status"];
            return View();
		}

        public IActionResult Details()
        {
            OrderVM orderVM;
            int Orderid;bool valid;
              valid = int.TryParse( HttpContext.Request.Query["id"], out Orderid);
            if(!valid)
            {
                TempData["Error"] = "Order Details Could Not Be Found ";
                RedirectToAction(nameof(Index));
            }
            orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == Orderid, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == Orderid, includeProperties: "Product")
            };

            return View(orderVM);


        }

        #region API CALLS
        [HttpGet]
       
        public IActionResult GetAll()
        {
            string status = HttpContext.Request.Query["status"];
            List<OrderHeader> OrderList;

            if (status == "All" || status == null)
			{
                OrderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
			else
			{
               
                OrderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").Where(x => x.OrderStatus == status).ToList();
            }

			
			return Json(new {data =  OrderList });

		}



		#endregion
	}
}
