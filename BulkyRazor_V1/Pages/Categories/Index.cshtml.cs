using BulkyRazor_V1.Data;
using BulkyRazor_V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazor_V1.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        
        public List<Category> categoryList{ get; set; }
        public IndexModel( ApplicationDbContext db)
        {
                _db = db;
        }

        public void OnGet()
        {
            categoryList = _db.Categories.ToList();
        }
    }
}
