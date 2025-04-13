using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReadingIsGood.Application.Attributes;
using ReadingIsGood.Application.Features.Books.Command.CreateBook;
using ReadingIsGood.Application.Features.Books.Query.GetBookList;
using ReadingIsGood.BuildingBlocks.Common.BaseController;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using ReadingIsGood.Domain.Constants;

namespace ReadingIsGood.Api.Controllers;

/// <summary>
/// Books related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[CustomAuthorize]
public class BookController : BaseController
{
    /// <summary>
    /// Create book
    /// </summary>
    [CustomAuthorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponse<CreateBookCommandResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("CreateBook")]
    public async ValueTask<IActionResult> CreateBook([FromBody] CreateBookCommandRequest request) => CreateActionResultInstance(await Mediator.Send(request));

    /// <summary>
    /// Get book list
    /// </summary>
    [CustomAuthorize]
    [ProducesResponseType(typeof(BaseResponse<GetBookListQueryResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("GetBookList")]
    public async ValueTask<IActionResult> GetBookList([FromBody] GetBookListQueryRequest request) => CreateActionResultInstance(await Mediator.Send(request));
}