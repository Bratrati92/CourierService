using CourierService.Core.Model;
using CourierService.DeliveryEstimation.Models;

namespace CourierService.DeliveryEstimation.Service
{
    public interface IDeliveryTimeCalculator
    {
        List<DeliveryEstimationResult> CalculateDeliveryTimes(List<DeliveryEstimationResult> results, List<Package> packages, VehicleDetils vehicleDetils);
    }
}