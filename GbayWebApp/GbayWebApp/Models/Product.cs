using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.Models
{
    public class Product
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        public string Description { get; set; }
    
        [Required]
        public decimal Price { get; set; }

        public string Seller { get; set; }

        [DataType(DataType.Url)]
        public string ImgUrl { get; set; }


    }
}
