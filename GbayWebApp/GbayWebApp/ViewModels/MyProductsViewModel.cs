using GbayWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.ViewModels
{
    public class MyProductsViewModel
    {
        public MyProductsViewModel()
        {
            this.ProductList = new List<Product>();
        }

        public List<Product> ProductList { get; set; }
    }
}
