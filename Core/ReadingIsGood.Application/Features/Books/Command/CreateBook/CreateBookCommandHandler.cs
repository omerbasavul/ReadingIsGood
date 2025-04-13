using MapsterMapper;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Models;
using ReadingIsGood.Application.Services.Books;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Books.Command.CreateBook;

public sealed class CreateBookCommandHandler(
    IBookService bookService,
    ILogger<CreateBookCommandHandler> logger,
    IMapper mapper
) : IRequestHandler<CreateBookCommandRequest, BaseResponse<CreateBookCommandResponse>>
{
    public async ValueTask<BaseResponse<CreateBookCommandResponse>> Handle(CreateBookCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var bookModel = mapper.Map<BookModel>(request);

            var created = await bookService.CreateBookAsync(bookModel);
            return BaseResponse<CreateBookCommandResponse>.Success(new CreateBookCommandResponse(created), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<CreateBookCommandResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(CreateBookCommandHandler)}");
            return BaseResponse<CreateBookCommandResponse>.Fail($"An exception occured while processing {nameof(CreateBookCommandHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}