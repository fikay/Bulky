using BulkyRazor_V1.Data;
using BulkyRazor_V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazor_V1.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
                _db = db;
        }
        public void OnGet(int? Id)
        {
           if(Id != null && Id != 0)
            {
                category = _db.Categories.Find(Id);
            }

            
           
        }
    }
}
