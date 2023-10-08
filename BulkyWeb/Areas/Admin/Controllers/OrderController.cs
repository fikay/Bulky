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
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Company)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            string status = HttpContext.Request.Query["status"];
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderVM orderVM;
            orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };
            return View(orderVM);
        }


        [HttpPost]
        [CustomAuthorizeAttribute(SD.Role_Admin, SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var OrderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            OrderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            OrderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            OrderHeaderFromDb.City = OrderVM.OrderHeader.City;
            OrderHeaderFromDb.State = OrderVM.OrderHeader.State;
            OrderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(OrderHeaderFromDb);
            _unitOfWork.save();

            TempData["success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), new { id = OrderHeaderFromDb.Id });


        }

        [HttpPost]
        [CustomAuthorizeAttribute(SD.Role_Admin, SD.Role_Employee)]
        public IActionResult OrderProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.save();
            TempData["Success"] = "Order Status Updated";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [CustomAuthorizeAttribute(SD.Role_Admin, SD.Role_Employee)]
        public IActionResult OrderShipping()
        {
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier) && !string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                _unitOfWork.OrderHeader.UpdateShipping(OrderVM.OrderHeader.Id, OrderVM.OrderHeader.Carrier, OrderVM.OrderHeader.TrackingNumber);
            }
            else
            {
                TempData["Error"] = "Please check that carrier and tracking number is not empty. Status not updated";
                return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
            }

            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
            _unitOfWork.save();
            TempData["Success"] = "Order Status Updated";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderCancellation = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            if (orderCancellation != null)
            {
                if (orderCancellation.PaymentStatus == SD.PaymentStatusApproved)
                {
                    var options = new RefundCreateOptions { PaymentIntent = OrderVM.OrderHeader.PaymentIntentId };
                    var service = new RefundService();
                    service.Create(options);

                    _unitOfWork.OrderHeader.UpdateStatus(orderCancellation.Id, SD.StatusCancelled, SD.StatusRefunded);
                }
                else
                {
                    _unitOfWork.OrderHeader.UpdateStatus(orderCancellation.Id, SD.StatusCancelled, SD.StatusCancelled);
                } 
            }

            _unitOfWork.save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });

        }

        [HttpPost]
        public IActionResult Payment()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");
            var domain = "https://localhost:7055/";
            //go to stripe
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?id={OrderVM.OrderHeader.Id}",
            };

            foreach (var cartItem in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cartItem.Price * 100),
                        Currency = "CAD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Product.Title,
                        },

                    },
                    Quantity = cartItem.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.updateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            
        }

        public IActionResult PaymentConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.updateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.save();
                }
            }
            return View(id);   
        }
        #region API CALLS
        [HttpGet]

        public IActionResult GetAll()
        {
            string status = HttpContext.Request.Query["status"];
            List<OrderHeader> OrderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                OrderList = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == UserId, includeProperties: "ApplicationUser").ToList();

            }

            if (status != "All" && status != null)
            {
                OrderList = OrderList.Where(x => x.OrderStatus == status).ToList();
            }
            //else
            //{

            //             OrderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").Where(x => x.OrderStatus == status).ToList();
            //         }


            return Json(new { data = OrderList });

        }



        #endregion

    }
}
