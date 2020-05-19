using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GbayWebApp.Models
{
    public class CreditOption
    {
        public CreditOption()
        {
            this.Options = new List<BuyCreditOption>();
        }
        public List<BuyCreditOption> Options { get; set; }
    }
}
