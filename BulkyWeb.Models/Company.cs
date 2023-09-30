using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength =3, ErrorMessage ="Company Name must be greater than or equal to  3")]
        public string ?Name { get; set; }

        [Display(Name ="Street Address")]
        public string ?StreetAddress { get; set; }
        public string? City { get; set; }
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$",  ErrorMessage ="Postal Code must match format xxx-xxx")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        public string? State { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone Number must match format xxx-xxx-xxxx")]
        public string ? PhoneNumber { get; set; }

    }
}
