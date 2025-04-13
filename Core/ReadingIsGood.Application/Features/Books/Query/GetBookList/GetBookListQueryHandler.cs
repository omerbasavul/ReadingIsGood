using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Books;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Books.Query.GetBookList;

public sealed class GetBookListQueryHandler(
    IBookService bookService,
    ILogger<GetBookListQueryHandler> logger
) : IRequestHandler<GetBookListQueryRequest, BaseResponse<GetBookListQueryResponse>>
{
    public async ValueTask<BaseResponse<GetBookListQueryResponse>> Handle(GetBookListQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize is < 1 or > 100) request.PageSize = 10;

            var (books, pagination) = await bookService.GetBookListAsync(request.PageNumber, request.PageSize);

            return BaseResponse<GetBookListQueryResponse>.Success(new GetBookListQueryResponse(books, pagination), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<GetBookListQueryResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(GetBookListQueryHandler)}");
            return BaseResponse<GetBookListQueryResponse>.Fail($"An exception occured while processing {nameof(GetBookListQueryHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}