using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Helpers;
using ReadingIsGood.Application.Services.Customers;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Authentication.Command.SignIn;

public sealed class SignInCommandHandler(
    ICustomerService customerService,
    ILogger<SignInCommandHandler> logger) : IRequestHandler<SignInCommandRequest, BaseResponse<SignInCommandResponse>>
{
    public async ValueTask<BaseResponse<SignInCommandResponse>> Handle(SignInCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            #region Email & Password Check

            if (string.IsNullOrEmpty(request.EmailAddress) || string.IsNullOrEmpty(request.Password))
                return BaseResponse<SignInCommandResponse>.Fail("Email Address or Password is cannot be empty", StatusCodes.Status401Unauthorized);

            #endregion

            var userModel = await customerService.AuthenticateAsync(request.EmailAddress, request.Password);
            var token = JwtTokenHelper.GenerateJwtToken(new DefaultJwtTokenModel(userModel.Id, userModel.Roles));

            return BaseResponse<SignInCommandResponse>.Success(new SignInCommandResponse(token), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<SignInCommandResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError($"An Exception Occured While Sign In User: {request.EmailAddress}, Exception Message: {ex.Message}");
            return BaseResponse<SignInCommandResponse>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }
}