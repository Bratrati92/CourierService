using CourierService.Core.Model;

namespace CourierService.Core.Service
{
    public interface IOfferService
    {
        double CalculateDiscountPercentage(Package package);
    }
}