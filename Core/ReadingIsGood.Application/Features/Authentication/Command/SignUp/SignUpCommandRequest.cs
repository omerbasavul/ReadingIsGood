using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Authentication.Command.SignUp;

public class SignUpCommandRequest : IRequest<BaseResponse<SignUpCommandResponse>>
{
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string Email { get; set; }
    public required string CountryCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string PasswordRepeat { get; set; }
}