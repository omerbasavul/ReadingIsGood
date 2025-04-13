using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrdersByDateRange;

public sealed class GetOrdersByDateRangeQueryRequest : IRequest<BaseResponse<GetOrdersByDateRangeQueryResponse>>
{
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}