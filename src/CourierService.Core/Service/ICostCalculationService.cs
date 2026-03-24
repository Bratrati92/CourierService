using CourierService.Core.Model;

namespace CourierService.Core.Service
{
    public interface ICostCalculationService
    {
        double CalculateDeliveryCost(Package package, double baseCost);
        double CalculateDiscount(double deliverycost, Package package);
    }
}