
using CourierService.Core.Model;
using CourierService.Core.Service;
using DeliveryEstimation.Models;
using DeliveryEstimation.Service;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var coupons = configuration.GetSection("Coupons").Get<List<Coupons>>();

var line = Console.ReadLine();
var parts = line.Split(' ');
double baseDeliveryCost = double.Parse(parts[0]);
int numberOfPackages = int.Parse(parts[1]);

var offerService = new OfferService(coupons);
var costCalculator = new CostCalculationService(offerService);
var packages = new List<Package>();
var deliveryEstimationResults = new List<DeliveryEstimationResult>();

for (int i = 0; i < numberOfPackages; i++)
{
    line = Console.ReadLine();
    parts = line.Split(' ');
    var package = ParsePackage(line);

    packages.Add(package);

    double deliveryCost = costCalculator.CalculateDeliveryCost(package, baseDeliveryCost);
    double discount = costCalculator.CalculateDiscount(deliveryCost, package);
    double totalCost = deliveryCost - discount;

    deliveryEstimationResults.Add(new DeliveryEstimationResult
    {
        PackageId = package.PackageId,
        Discount = discount,
        TotalCost = totalCost
    });
}

Package ParsePackage(string line)
{
    var parts = line.Trim().Split(' ');
    return new Package
    {
        PackageId = parts[0],
        Weight = double.Parse(parts[1]),
        Distance = double.Parse(parts[2]),
        OfferCode = parts.Length > 3 ? parts[3] : string.Empty
    };
}

VehicleDetils ParseVechicle(string line)
{
    var parts = line.Trim().Split(' ');
    return new VehicleDetils
    {
        NoOfVehicles = int.Parse(parts[0]),
        MaxSpeed = double.Parse(parts[1]),
        MaxCarryWeight = double.Parse(parts[2]),
    };
}

line = Console.ReadLine();
parts = line.Split(' ');

var vechileDetails = ParseVechicle(line);

var timeCalculator = new DeliveryTimeCalculator();
timeCalculator.CalculateDeliveryTimes(deliveryEstimationResults, packages, vechileDetails);

foreach (var result in deliveryEstimationResults)
{
    Console.WriteLine(result);
}