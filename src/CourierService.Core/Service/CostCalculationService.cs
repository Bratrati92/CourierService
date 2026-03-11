using CourierService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Core.Service
{
    public class CostCalculationService : ICostCalculationService
    {
        public double CalculateDeliveryCost(Package package, double baseCost)
        {
            return baseCost + (package.Weight * 10) + (package.Distance * 5);
        }

        public double CalculateDiscount(double deliverycost, Package package, IOfferService offerService)
        {
            double discount = offerService.CalculateDiscountPercentage(package);
            return (deliverycost * discount / 100);
        }


    }
}
