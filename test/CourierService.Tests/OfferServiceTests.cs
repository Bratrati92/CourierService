using CourierService.Core.Model;
using CourierService.Core.Service;
namespace CourierService.Tests
{
    public class OfferServiceTests
    {

        private readonly IOfferService _offerService;

        public OfferServiceTests()
        {
            var coupons = new List<Coupons>
            {
                new Coupons { OfferCode = "OFR001", Discount = 10, MinWeight = 70, MaxWeight = 200, MinDistance = 0, MaxDistance = 199 },
                new Coupons { OfferCode = "OFR002", Discount = 7, MinWeight = 100, MaxWeight = 250, MinDistance = 50, MaxDistance = 150 },
                new Coupons { OfferCode = "OFR003", Discount = 5, MinWeight = 10, MaxWeight = 150, MinDistance = 50, MaxDistance = 250 }
            };
            _offerService = new OfferService(coupons);
        }



        [Theory]
        [InlineData("OFR001", 50, 5)]
        [InlineData("OFR003", 50, 5)]
        public void CalculateDiscount_InvalidOffer_ReturnsZero(string offerCode, double weight, double distance)
        {
            var package = new Package { PackageId = "PKG1", Weight = weight, Distance = distance, OfferCode = offerCode };
            double discount = _offerService.CalculateDiscountPercentage(package);
            Assert.Equal(0, discount);

        }

        [Fact]
        public void CalculateDiscount_ValidOffer_ReturnsDiscount()
        {
            var package = new Package { PackageId = "PKG1", Weight = 80, Distance = 150, OfferCode = "OFR001" };
            double discount = _offerService.CalculateDiscountPercentage(package);
            Assert.Equal(10, discount);
        }

        [Theory]
        [InlineData("OFR010", 50, 5)]
        [InlineData("OFR013", 50, 5)]
        public void CalculateDiscount_UnknownCoupon_ReturnsZero(string offerCode, double weight, double distance)
        {
            var package = new Package { PackageId = "PKG1", Weight = weight, Distance = distance, OfferCode = offerCode };
            double discount = _offerService.CalculateDiscountPercentage(package);
            Assert.Equal(0, discount);

        }


    }
}