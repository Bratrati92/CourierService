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

            int n = candidates.Count;
            if (n == 0) return bestSubset;

            int[] weight = candidates.Select(w => (int)(w.Weight * 100)).ToArray();
            double[] distance = candidates.Select(p => p.Distance).ToArray();
            int capacity = (int)(maxWeight * 100);

            int[,] dp = new int[n + 1, capacity + 1];        // max weight
            int[,] cnt = new int[n + 1, capacity + 1];       // package count
            double[,] mxd = new double[n + 1, capacity + 1]; // max distance
            bool[,] took = new bool[n + 1, capacity + 1];    // was item taken?

            for (int idx = 1; idx <= n; idx++)
            {
                for (int w = 0; w <= capacity; w++)
                {
                    // Default: skip item
                    dp[idx, w] = dp[idx - 1, w];
                    cnt[idx, w] = cnt[idx - 1, w];
                    mxd[idx, w] = mxd[idx - 1, w];

                    if (weight[idx - 1] <= w)
                    {
                        int pw = w - weight[idx - 1];
                        int takeWt = dp[idx - 1, pw] + weight[idx - 1];
                        int takeCnt = cnt[idx - 1, pw] + 1;
                        double takeDist = Math.Max(mxd[idx - 1, pw], distance[idx - 1]);

                        if (IsBetter(takeCnt, takeWt, takeDist,
                                     cnt[idx, w], dp[idx, w], mxd[idx, w]))
                        {
                            dp[idx, w] = takeWt;
                            cnt[idx, w] = takeCnt;
                            mxd[idx, w] = takeDist;
                            took[idx, w] = true;
                        }
                    }
                }
            }

            // Backtrack using the took table
            for (int i = n, j = capacity; i > 0; i--)
            {
                if (took[i, j])
                {
                    bestSubset.Add(candidates[i - 1]);
                    j -= weight[i - 1];
                }
            }

            return bestSubset;
        }

        // Tie-breaking: more packages → higher weight → shorter max distance
        private static bool IsBetter(int countA, int weightA, double distA,
                                      int countB, int weightB, double distB)
        {
            if (countA > countB) return true;
            if (countA < countB) return false;
            if (weightA > weightB) return true;
            if (weightA < weightB) return false;
            return distA < distB;
        }

       


    }
}
