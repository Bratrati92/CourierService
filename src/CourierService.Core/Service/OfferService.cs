using CourierService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Core.Service
{
    public class OfferService : IOfferService
    {
        List<Coupons> coupons = new List<Coupons>()
        {
            new Coupons(){ OfferCode = "OFR001", Discount = 10, MinWeight = 70, MaxWeight = 200, MinDistance = 0, MaxDistance = 199 },
            new Coupons(){ OfferCode = "OFR002", Discount = 7, MinWeight = 100, MaxWeight = 250, MinDistance = 50, MaxDistance = 150 },
            new Coupons(){ OfferCode = "OFR003", Discount = 5, MinWeight = 10, MaxWeight = 150, MinDistance = 50, MaxDistance = 250 }
        };

        public double CalculateDiscountPercentage(Package package)
        {
            var coupon = coupons.FirstOrDefault(c => c.OfferCode == package.OfferCode);
            if (isCouponValid(package, coupon))
            {
                return coupon.Discount;
            }
            return 0;
        }

        private static bool isCouponValid(Package package, Coupons? coupon)
        {
            return coupon != null && package.Weight >= coupon.MinWeight && package.Weight <= coupon.MaxWeight
                            && package.Distance >= coupon.MinDistance && package.Distance <= coupon.MaxDistance;
        }
    }
}
