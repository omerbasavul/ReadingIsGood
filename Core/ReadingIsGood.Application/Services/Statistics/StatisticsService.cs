using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;
using ReadingIsGood.Domain.Entities;

namespace ReadingIsGood.Application.Services.Statistics;

public sealed class StatisticsService(
    IBaseRepository<Order, Guid> orderRepository
) : IStatisticsService
{
    public async ValueTask<List<MonthlyStatisticsModel>> GetMonthlyStatisticsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.Set()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
            throw new CustomServiceException("No orders found for this customer", StatusCodes.Status404NotFound);

        var grouped = orders
            .GroupBy(o => new { o.CreatedDate?.Year, o.CreatedDate?.Month })
            .Select(g =>
            {
                int totalOrderCount = g.Count();

                int totalBookCount = g.Sum(order =>
                    order.OrderItems?.Sum(item => item.Quantity) ?? 0
                );

                decimal totalPurchasedAmount = g.Sum(x => x.TotalAmount);

                return new MonthlyStatisticsModel
                {
                    Year = (int)g.Key.Year!,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)g.Key.Month!),
                    TotalOrderCount = totalOrderCount,
                    TotalBookCount = totalBookCount,
                    TotalPurchasedAmount = totalPurchasedAmount
                };
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => DateTime.ParseExact(x.MonthName, "MMMM", CultureInfo.CurrentCulture).Month)
            .ToList();

        return grouped;
    }
}