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
        private readonly List<Coupons> _coupons;

        public OfferService(List<Coupons> coupons)
        {
            _coupons = coupons;
        }

        public double CalculateDiscountPercentage(Package package)
        {
            var coupon = _coupons.FirstOrDefault(c => c.OfferCode == package.OfferCode);
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
