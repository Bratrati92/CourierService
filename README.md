# CourierService

A .NET 8 console application for a courier service that calculates delivery costs with offer-based discounts and estimates delivery times across a vehicle fleet.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Solution Structure

```
CourierService.sln
├── src/
│   ├── CourierService.Core/          # Shared class library
│   │   ├── Model/
│   │   │   ├── Coupons.cs            # Offer code definition (discount %, weight/distance range)
│   │   │   ├── Package.cs            # Input package (id, weight, distance, offer code)
│   │   │   ├── Result.cs             # Cost calculation output (id, discount, total cost)
│   │   │   └── VehicleDetils.cs      # Fleet config (vehicle count, speed, capacity)
│   │   └── Service/
│   │       ├── ICostCalculationService.cs
│   │       ├── CostCalculationService.cs
│   │       ├── IOfferService.cs
│   │       └── OfferService.cs
│   ├── CostCalculation/              # Console app — cost + discount
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── DeliveryEstimation/           # Console app — cost + discount + delivery time
│       ├── Program.cs
│       ├── appsettings.json
│       ├── Models/
│       │   └── DeliveryEstimationResult.cs
│       └── Service/
│           ├── IDeliveryTimeCalculator.cs
│           └── DeliveryTimeCalculator.cs
└── test/
    └── CourierService.Tests/          # xUnit test project
        ├── CostCalculationServiceTests.cs
        ├── OfferServiceTests.cs
        └── DeliveryTimeCalculatorTests.cs
```

## Projects

| Project | Type | Description |
|---|---|---|
| **CourierService.Core** | Class Library | Shared models and services — cost calculation, offer/coupon logic |
| **CourierService.CostEstimation** | Console App | Outputs per-package discount and total cost |
| **CourierService.DeliveryEstimation** | Console App | Additionally estimates delivery time using vehicle fleet details |
| **CourierService.Tests** | xUnit Tests | 13 tests covering cost calculation, offers, and delivery time |

## Dependencies

| Package | Version | Used By |
|---|---|---|
| Microsoft.Extensions.Configuration.Json | 8.0.1 | CostEstimation, DeliveryEstimation |
| Microsoft.Extensions.Configuration.Binder | 8.0.2 | CostEstimation, DeliveryEstimation |
| xunit | 2.5.3 | Tests |
| xunit.runner.visualstudio | 2.5.3 | Tests |
| Microsoft.NET.Test.Sdk | 17.8.0 | Tests |
| coverlet.collector | 6.0.0 | Tests |

## Cost Formula

```
Delivery Cost = Base Cost + (Weight in kg × 10) + (Distance in km × 5)
```

Discount is applied as a percentage of the delivery cost when a valid offer code is provided and the package meets the offer's weight and distance criteria.

## Available Offer Codes

Offer codes are configured in `appsettings.json` and loaded at runtime:

| Code | Discount | Distance (km) | Weight (kg) |
|---|---|---|---|
| OFR001 | 10% | 0–199 | 70–200 |
| OFR002 | 7% | 50–150 | 100–250 |
| OFR003 | 5% | 50–250 | 10–150 |

A coupon is valid only when **both** the package weight and distance fall within the specified ranges (inclusive). An unknown or unmatched code results in zero discount.

## Build & Run

### Build the entire solution

```bash
dotnet build
```

### Cost Estimation

```bash
cd src/CostCalculation
dotnet run
```

**Input:**
```
100 3
PKG1 5 5 OFR001
PKG2 15 5 OFR002
PKG3 10 100 OFR003
```

- Line 1: `<base_delivery_cost> <number_of_packages>`
- Subsequent lines: `<package_id> <weight_kg> <distance_km> <offer_code>`

**Output:**
```
PKG1 0.00 175.00
PKG2 0.00 275.00
PKG3 35.00 665.00
```

Each line: `<package_id> <discount> <total_cost>`

### Delivery Time Estimation

```bash
cd src/DeliveryEstimation
dotnet run
```

**Input:**
```
100 5
PKG1 50 30 OFR001
PKG2 75 125 OFR002
PKG3 175 100 OFR003
PKG4 110 60 OFR002
PKG5 155 95 NA
2 70 200
```
- Line 1: `<base_delivery_cost> <number_of_packages>`
- Subsequent lines: `<package_id> <weight_kg> <distance_km> <offer_code>`
- Last line: `<number_of_vehicles> <max_speed_km_hr> <max_carriable_weight_kg>`

**Output:**
```
PKG1 0.00 750.00 3.98
PKG2 0.00 1475.00 1.78
PKG3 35.00 1395.00 1.42
PKG4 105.00 1395.00 0.85
PKG5 0.00 2350.00 4.19
```

Each line: `<package_id> <discount> <total_cost> <estimated_delivery_hours>`

## Delivery Time Algorithm

1. Finds the earliest available vehicle.
2. Uses a **dynamic-programming (knapsack) approach** to select the best combination of undelivered packages within the vehicle's weight limit, preferring:
   - More packages first
   - Higher total weight on ties
   - Shorter maximum distance as the final tiebreaker
3. Delivers all packages in the shipment (each at `distance / speed` hours from departure).
4. Vehicle returns after a round trip of the farthest package in the shipment.
5. Repeats until all packages are delivered.

Delivery times are truncated (floored) to 2 decimal places.

## Running Tests

```bash
dotnet test
```

### Test Coverage

| Test Class | Tests | Covers |
|---|---|---|
| **CostCalculationServiceTests** | 5 | Delivery cost formula, discount with valid/invalid offers |
| **OfferServiceTests** | 4 | Coupon validation — weight/distance ranges, unknown codes |
| **DeliveryTimeCalculatorTests** | 4 | Single/multi-vehicle routing, load balancing, tiebreaking |

## Configuration

Both console apps load offer codes from `appsettings.json` at startup. To add or modify offers, edit the `Coupons` array:

```json
{
  "Coupons": [
    {
      "OfferCode": "OFR001",
      "Discount": 10,
      "MinWeight": 70,
      "MaxWeight": 200,
      "MinDistance": 0,
      "MaxDistance": 199
    }
  ]
}
```

Each coupon entry defines the offer code, discount percentage, and the valid weight/distance ranges.

## Build & Test

```bash
dotnet build
dotnet test
```
