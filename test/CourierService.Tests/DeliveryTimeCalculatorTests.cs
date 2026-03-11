using CourierService.Core.Model;
using DeliveryEstimation.Models;
using DeliveryEstimation.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Tests
{
    public  class DeliveryTimeCalculatorTests
    {
        private readonly DeliveryTimeCalculator _calculator;

        public DeliveryTimeCalculatorTests()
        {
            _calculator = new DeliveryTimeCalculator();
        }

        [Fact]
        public void CalculateDeliveryTimes_SinglePackageSingleVehicle_CorrectTime()
        {
            var packages = new List<Package>
        {
            new() { PackageId = "PKG1", Weight = 50, Distance = 70 }
        };
            var results = new List<DeliveryEstimationResult>
        {
            new() { PackageId = "PKG1", Discount = 0, TotalCost = 750 }
        };
            var vehicleDetils = new VehicleDetils { NoOfVehicles = 1, MaxSpeed = 70, MaxCarryWeight = 200 };

            _calculator.CalculateDeliveryTimes(results, packages, vehicleDetils);

            // 70km / 70km/hr = 1.00 hr
            Assert.Equal(1.00, results[0].deliveryEstimationTime);
        }

        [Fact]
        public void CalculateDeliveryTimes_TwoVehicles_DistributesLoad()
        {
            var packages = new List<Package>
        {
            new() { PackageId = "PKG1", Weight = 100, Distance = 70 },
            new() { PackageId = "PKG2", Weight = 80, Distance = 140 },
            new() { PackageId = "PKG3", Weight = 150, Distance = 35 },
            new() { PackageId = "PKG4", Weight = 50, Distance = 105 },
            new() { PackageId = "PKG5", Weight = 110, Distance = 84 }
        };
            var results = packages.Select(p => new DeliveryEstimationResult
            {
                PackageId = p.PackageId,
                Discount = 0,
                TotalCost = 0
            }).ToList();
            var vehicleDetils = new VehicleDetils { NoOfVehicles = 2, MaxSpeed = 70, MaxCarryWeight = 200 };

            _calculator.CalculateDeliveryTimes(results, packages, vehicleDetils);

            var resultMap = results.ToDictionary(r => r.PackageId);
            // Trip 1 — Vehicle 0 picks PKG3(150)+PKG4(50)=200kg heaviest combo
            //   PKG3: 35/70=0.50hr, PKG4: 105/70=1.50hr, returns at 3.00hr
            // Trip 1 — Vehicle 1 picks PKG2(80)+PKG5(110)=190kg
            //   PKG2: 140/70=2.00hr, PKG5: 84/70=1.20hr, returns at 4.00hr
            // Trip 2 — Vehicle 0 (available at 3.00) picks PKG1(100kg)
            //   PKG1: 70/70=1.00hr, arrives at 3.00+1.00=4.00hr
            Assert.Equal(4.00, resultMap["PKG1"].deliveryEstimationTime);
            Assert.Equal(2.00, resultMap["PKG2"].deliveryEstimationTime);
            Assert.Equal(0.50, resultMap["PKG3"].deliveryEstimationTime);
            Assert.Equal(1.50, resultMap["PKG4"].deliveryEstimationTime);
            Assert.Equal(1.20, resultMap["PKG5"].deliveryEstimationTime);
        }
    }
}
