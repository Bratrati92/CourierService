using CourierService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryEstimation.Models
{
    public  class DeliveryEstimationResult : Result
    {
        public double deliveryEstimationTime { get; set; }

        public override string ToString()
        {
            return $"{PackageId} {Discount:F2} {TotalCost:F2} {deliveryEstimationTime:F2} ";
        }

    }
}
