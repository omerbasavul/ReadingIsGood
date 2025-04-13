namespace ReadingIsGood.Application.Models;

public class MonthlyStatisticsModel
{
    public string MonthName { get; set; }
    public int Year { get; set; }
    public int TotalOrderCount { get; set; }
    public int TotalBookCount { get; set; }
    public decimal TotalPurchasedAmount { get; set; }
}
