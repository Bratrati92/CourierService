
using CourierService.Core.Model;
using CourierService.Core.Service;

var line = Console.ReadLine();
var parts = line.Trim().Split(' ');
double basedeliverycost = double.Parse(parts[0]);
int numberofpackage = int.Parse(parts[1]);

var offerService = new OfferService();
var costCalculationService = new CostCalculationService();
List<Result> costEstimationResult = new List<Result>();

for (int i = 0; i < numberofpackage; i++)
{
    line = Console.ReadLine();
    Package package = ParsePackage(line);
   
    double deliverycost = costCalculationService.CalculateDeliveryCost(package,basedeliverycost) ;
    Console.WriteLine($"Delivery cost for package {package.PackageId}: {deliverycost}");
    double discount = costCalculationService.CalculateDiscount(deliverycost,package,offerService);
    Console.WriteLine($"Discount for package {package.PackageId}: {discount}");
    double totalcost = deliverycost - discount;

    costEstimationResult.Add(new Result { PackageId = package.PackageId,
        Discount = discount,
        TotalCost = totalcost
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

foreach (var result in costEstimationResult)
{
    Console.WriteLine(result);
}

