using CourierService.Core.Model;
using DeliveryEstimation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryEstimation.Service
{
    public class DeliveryTimeCalculator : IDeliveryTimeCalculator
    {
        public List<DeliveryEstimationResult> CalculateDeliveryTimes(
       List<DeliveryEstimationResult> results,
       List<Package> packages,
       VehicleDetils vehicleDetils)
        {
            var resultMap = results.ToDictionary(r => r.PackageId);
            var packageMap = packages.ToDictionary(p => p.PackageId);

            var undelivered = new HashSet<string>(packages.Select(p => p.PackageId));
            var vehicleAvailableAt = new double[vehicleDetils.NoOfVehicles];

            while (undelivered.Count > 0)
            {
                // Find the vehicle that becomes available earliest
                int vehicleIndex = 0;
                for (int i = 1; i < vehicleDetils.NoOfVehicles; i++)
                {
                    if (vehicleAvailableAt[i] < vehicleAvailableAt[vehicleIndex])
                        vehicleIndex = i;
                }
                double currentTime = vehicleAvailableAt[vehicleIndex];

                // Find the heaviest combination of packages within max weight
                var bestShipment = FindBestShipment(
                    undelivered.Select(id => packageMap[id]).ToList(),
                    vehicleDetils.MaxCarryWeight);

                if (bestShipment.Count == 0) break;

                double maxTripTime = 0;
                foreach (var pkg in bestShipment)
                {
                    double deliveryTime = Math.Floor(pkg.Distance / vehicleDetils.MaxSpeed * 100) / 100.0;
                    double arrivalTime = currentTime + deliveryTime;

                    resultMap[pkg.PackageId].deliveryEstimationTime = arrivalTime;

                    if (deliveryTime > maxTripTime)
                        maxTripTime = deliveryTime;

                    undelivered.Remove(pkg.PackageId);
                }

                // Vehicle returns after round trip of the farthest package
                vehicleAvailableAt[vehicleIndex] = currentTime + (2 * maxTripTime);
            }

            return results;
        }

        private static List<Package> FindBestShipment(List<Package> candidates, double maxWeight)
        {
            var bestSubset = new List<Package>();
            double bestWeight = 0;

            int n = candidates.Count;
            for (int mask = 1; mask < (1 << n); mask++)
            {
                var subset = new List<Package>();
                double totalWeight = 0;

                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        subset.Add(candidates[i]);
                        totalWeight += candidates[i].Weight;
                    }
                }

                if (totalWeight > maxWeight) continue;

                bool isBetter = false;
                if (totalWeight > bestWeight)
                {
                    isBetter = true;
                }
                else if (Math.Abs(totalWeight - bestWeight) < 0.001)
                {
                    if (subset.Count < bestSubset.Count)
                    {
                        isBetter = true;
                    }
                    else if (subset.Count == bestSubset.Count)
                    {
                        double maxDist = subset.Max(p => p.Distance);
                        double bestMaxDist = bestSubset.Max(p => p.Distance);
                        if (maxDist < bestMaxDist)
                            isBetter = true;
                    }
                }

                if (isBetter)
                {
                    bestSubset = subset;
                    bestWeight = totalWeight;
                }
            }

            return bestSubset;
        }

    }
}
