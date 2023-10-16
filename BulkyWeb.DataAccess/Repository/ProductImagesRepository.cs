using BulkyWeb.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class ProductImagesRepository :Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;

        public ProductImagesRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        
        public void Update(ProductImage obj)
        {
           _db.ProductImages.Update(obj);
        }
    }
}
