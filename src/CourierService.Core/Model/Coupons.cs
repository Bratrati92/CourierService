using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Core.Model
{
    public class Coupons
    {
        public string OfferCode { get; set; }
        public double Discount { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
    }
}
