using System.Net;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using ReadingIsGood.Application.Attributes;
using ReadingIsGood.Application.Features.Customers.Command.SignIn;
using ReadingIsGood.Application.Features.Customers.Command.SignUp;
using ReadingIsGood.Application.Features.Customers.Query.GetCustomerOrderList;
using ReadingIsGood.BuildingBlocks.Common.BaseController;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Api.Controllers;

/// <summary>
/// Customers related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
public class CustomerController(IMapper mapper) : BaseController
{
    /// <summary>
    /// Customer login
    /// </summary>
    [ProducesResponseType(typeof(BaseResponse<SignInCommandResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("SignIn")]
    public async ValueTask<IActionResult> SignIn([FromBody] SignInCommandRequest request) => CreateActionResultInstance(await Mediator.Send(request));

    /// <summary>
    /// Customer register
    /// </summary>
    [ProducesResponseType(typeof(BaseResponse<SignUpCommandResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("SignUp")]
    public async ValueTask<IActionResult> SignUp([FromBody] SignUpCommandRequest request) => CreateActionResultInstance(await Mediator.Send(request));

    /// <summary>
    /// Get customer order list
    /// </summary>
    [CustomAuthorize]
    [ProducesResponseType(typeof(BaseResponse<GetCustomerOrderListQueryResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpPost("GetCustomerOrderList")]
    public async ValueTask<IActionResult> GetCustomerOrderList([FromBody] GetCustomerOrderListQueryRequest request)
    {
        var innerCommand = mapper.Map<GetCustomerOrderListInnerQueryRequest>(request);
        innerCommand.CustomerId = UserId;

        return CreateActionResultInstance(await Mediator.Send(innerCommand));
    }
}