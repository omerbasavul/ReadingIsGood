using System.Net;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using ReadingIsGood.Application.Attributes;
using ReadingIsGood.Application.Features.Orders.Command.CreateOrder;
using ReadingIsGood.Application.Features.Orders.Query.GetOrderDetailById;
using ReadingIsGood.Application.Features.Orders.Query.GetOrdersByDateRange;
using ReadingIsGood.BuildingBlocks.Common.BaseController;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using ReadingIsGood.Domain.Constants;

namespace ReadingIsGood.Api.Controllers;

/// <summary>
/// Orders related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[CustomAuthorize]
public class OrderController(IMapper mapper) : BaseController
{
    /// <summary>
    /// Create Order
    /// </summary>
    [CustomAuthorize]
    [ProducesResponseType(typeof(BaseResponse<CreateOrderCommandResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("CreateOrder")]
    public async ValueTask<IActionResult> CreateOrder([FromBody] CreateOrderCommandRequest request)
    {
        var innerCommand = mapper.Map<CreateOrderInnerCommandRequest>(request);
        innerCommand.CustomerId = UserId;

        return CreateActionResultInstance(await Mediator.Send(innerCommand));
    }

    /// <summary>
    /// Get Order by id
    /// </summary>
    [CustomAuthorize]
    [ProducesResponseType(typeof(BaseResponse<GetOrderDetailByIdQueryResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("GetOrderDetailById")]
    public async ValueTask<IActionResult> GetOrderList([FromBody] GetOrderDetailByIdQueryRequest request) => CreateActionResultInstance(await Mediator.Send(request));

    /// <summary>
    /// Get Orders by date range
    /// </summary>
    [CustomAuthorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponse<GetOrdersByDateRangeQueryResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("GetOrdersByDateRange")]
    public async ValueTask<IActionResult> GetOrdersByDateRange([FromBody] GetOrdersByDateRangeQueryRequest request) => CreateActionResultInstance(await Mediator.Send(request));
}