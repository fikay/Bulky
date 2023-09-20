using BulkyRazor_V1.Data;
using BulkyRazor_V1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyRazor_V1.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {    
                _db.Categories.Add(category);
                _db.SaveChanges();
                return RedirectToPage("Index");  
        }
    }
}
