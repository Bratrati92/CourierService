# CourierService

A .NET 8 console application for a courier service that calculates delivery costs with offer-based discounts and estimates delivery times across a vehicle fleet.

## Projects

| Project | Description |
|---|---|
| **CourierService.Core** | Shared models and services — cost calculation, offer/coupon logic |
| **CostCalculation** | Console app that outputs per-package discount and total cost |
| **DeliveryEstimation** | Console app that additionally estimates delivery time using  vehicle details  |
| **CourierService.Tests** | xUnit tests for cost calculation, delivery time and offers |

## Cost Formula

```
Delivery Cost = Base Cost + (Weight in kg × 10) + (Distance in km × 5)
```

Discount is applied as a percentage of the delivery cost when a valid offer code is provided and the package meets the offer's weight and distance criteria.

## Available Offer Codes

| Code | Discount | Distance (km) | Weight (kg) |
|---|---|---|---|
| OFR001 | 10% | 0–200 | 70–200 |
| OFR002 | 7% | 50–150 | 100–250 |
| OFR003 | 5% | 50–250 | 10–150 |

## Usage

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
2. Selects the heaviest combination of undelivered packages within the vehicle's weight limit.
3. Delivers all packages in the shipment (each at `distance / speed` hours from departure).
4. Vehicle returns after a round trip of the farthest package in the shipment.
5. Repeats until all packages are delivered.

Delivery times are truncated (floored) to 2 decimal places.

## Build & Test

```bash
dotnet build
dotnet test
```
