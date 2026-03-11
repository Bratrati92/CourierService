using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Core.Model
{
    public class Result
    {
        public string PackageId { get; set; }
        public double Discount { get; set; }
        public double TotalCost { get; set; }

        public override string ToString()
        {
            return $"{PackageId} {Discount:F2} {TotalCost:F2}";
        }
    }

    
}
