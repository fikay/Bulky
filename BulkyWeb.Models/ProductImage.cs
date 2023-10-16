﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        public Product Product { get; set; }

        [Required]
       public string ImageUrl { get; set; }
    }
}
