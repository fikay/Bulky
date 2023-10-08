using BulkyWeb.Data;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader> 
    {
        void Update(OrderHeader obj);

        void UpdateShipping(int id, string carrier, string trackingNumber);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void updateStripePaymentID(int id, string sessionId, string paymentIntentId);

        //void CancelOrder(int id, string orderStatus, string paymentStatus);
    }
}
