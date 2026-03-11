using CourierService.Core.Model;
using CourierService.Core.Service;
namespace CourierService.Tests
{
    public class OfferServiceTests
    {

        private readonly IOfferService _offerService;

        public OfferServiceTests()
        {
            _offerService = new OfferService();
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