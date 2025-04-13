using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Customers.Query.GetCustomerOrderList;

public class GetCustomerOrderListQueryRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public sealed class GetCustomerOrderListInnerQueryRequest : GetCustomerOrderListQueryRequest, IRequest<BaseResponse<GetCustomerOrderListQueryResponse>>
{
    public required Guid CustomerId { get; set; }
}