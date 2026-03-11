using CourierService.Core.Model;
using DeliveryEstimation.Models;

namespace DeliveryEstimation.Service
{
    public interface IDeliveryTimeCalculator
    {
        List<DeliveryEstimationResult> CalculateDeliveryTimes(List<DeliveryEstimationResult> results, List<Package> packages, VehicleDetils vehicleDetils);
    }
}