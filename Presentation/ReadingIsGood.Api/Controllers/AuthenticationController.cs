using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReadingIsGood.Application.Features.Authentication.Command.SignIn;
using ReadingIsGood.Application.Features.Authentication.Command.SignUp;
using ReadingIsGood.BuildingBlocks.Common.BaseController;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Api.Controllers;

/// <summary>
/// Authentication related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AuthenticationController : BaseController
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
}