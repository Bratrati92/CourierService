using CourierService.Core.Model;
using CourierService.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Tests
{
    public class CostCalculationServiceTests
    {
        private readonly ICostCalculationService _costCalculationService;
        public CostCalculationServiceTests()
        { 
            _costCalculationService = new CostCalculationService();
        }

        [Fact]
        public void CalculateDeliveryCost_ReturnsCorrectCost()
        {
            var package = new Package { PackageId = "PKG1", Weight = 5, Distance = 5 };

            double cost = _costCalculationService.CalculateDeliveryCost(package,100);

            // 100 + (5 * 10) + (5 * 5) = 100 + 50 + 25 = 175
            Assert.Equal(175, cost);
        }

        [Fact]
        public void CalculateDeliveryCost_HeavyAndFar_ReturnsCorrectCost()
        {
            var package = new Package { PackageId = "PKG3", Weight = 10, Distance = 100 };

            double cost = _costCalculationService.CalculateDeliveryCost(package,100);

            // 100 + (10 * 10) + (100 * 5) = 100 + 100 + 500 = 700
            Assert.Equal(700, cost);
        }

        [Fact]
        public void CalculateDiscount_ValidOffer_ReturnsDiscount()
        {            
            var coupons = new List<Coupons>
            {
                new Coupons { OfferCode = "OFR003", Discount = 5, MinWeight = 10, MaxWeight = 150, MinDistance = 50, MaxDistance = 250 }
            };
            var offerService = new OfferService(coupons);
            // OFR003: 5% discount, distance 50-250, weight 10-150
            var package = new Package { PackageId = "PKG3", Weight = 10, Distance = 100, OfferCode = "OFR003" };
            double deliveryCost = 700;

            double discount = _costCalculationService.CalculateDiscount(deliveryCost, package, offerService);

            // 5% of 700 = 35
            Assert.Equal(35, discount);
        }

        [Theory]
        [InlineData("OFR001", 5, 5, 175)]
        [InlineData("INVALID", 55, 55, 175)]
        public void CalculateDiscount_ReturnsZero(string offerCode, double weight, double distance, double deliveryCost)
        {
            var coupons = new List<Coupons>
            {
                new Coupons { OfferCode = "OFR003", Discount = 5, MinWeight = 10, MaxWeight = 150, MinDistance = 50, MaxDistance = 250 }
            };
            var offerService = new OfferService(coupons);
            var package = new Package { PackageId = "PKG1", Weight = weight, Distance = distance, OfferCode = offerCode };

            double discount = _costCalculationService.CalculateDiscount(deliveryCost, package, offerService);

            Assert.Equal(0, discount);
        }

    }
}
