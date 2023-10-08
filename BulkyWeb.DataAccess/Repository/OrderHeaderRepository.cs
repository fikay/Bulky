using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Utility;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository( ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.Update(obj);
        }

       void IOrderHeaderRepository.UpdateShipping(int id, string carrier, string trackingNumber)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(carrier))
                {
                    orderFromDb.Carrier = carrier;
                }
                if (!string.IsNullOrEmpty(trackingNumber))
                {
                    orderFromDb.TrackingNumber = trackingNumber;
                }

                if(orderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    orderFromDb.ShippingDate = DateTime.Now;
                    orderFromDb.PaymentDueDate =DateOnly.FromDateTime(DateTime.Now.AddDays(30));
                }
            }
        }

        void IOrderHeaderRepository.UpdateStatus(int id, string orderStatus, string? paymentStatus )
		{
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
		}

		void IOrderHeaderRepository.updateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
                

			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
