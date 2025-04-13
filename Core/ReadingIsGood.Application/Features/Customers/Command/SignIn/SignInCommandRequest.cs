using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Customers.Command.SignIn;

public sealed class SignInCommandRequest : IRequest<BaseResponse<SignInCommandResponse>>
{
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
}