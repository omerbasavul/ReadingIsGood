using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Books.Query.GetBookList;

public sealed class GetBookListQueryRequest : IRequest<BaseResponse<GetBookListQueryResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}