using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Helpers;
using ReadingIsGood.Application.Models;
using ReadingIsGood.Application.Services.Customers;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Customers.Command.SignUp;

public class SignUpCommandHandler(
    ICustomerService customerService,
    ILogger<SignUpCommandHandler> logger) : IRequestHandler<SignUpCommandRequest, BaseResponse<SignUpCommandResponse>>
{
    public async ValueTask<BaseResponse<SignUpCommandResponse>> Handle(SignUpCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            #region Validation

            if (!ValidationHelpers.IsValidPassword(request.Password))
                return BaseResponse<SignUpCommandResponse>.Fail("Password length must be greater than 8 characters and must contains at least one upper, one lower, one number and one special character.", StatusCodes.Status400BadRequest);

            if (request.Password != request.PasswordRepeat)
                return BaseResponse<SignUpCommandResponse>.Fail("Passwords not match.", StatusCodes.Status400BadRequest);

            if (string.IsNullOrWhiteSpace(request.Email) || !ValidationHelpers.IsValidEmail(request.Email))
                return BaseResponse<SignUpCommandResponse>.Fail("Invalid e-mail.", StatusCodes.Status400BadRequest);

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && !ValidationHelpers.IsValidPhoneNumber(request.PhoneNumber))
                return BaseResponse<SignUpCommandResponse>.Fail("Please enter valid phone number.", StatusCodes.Status400BadRequest);

            if (!string.IsNullOrWhiteSpace(request.CountryCode) && !ValidationHelpers.IsValidCountryCode(request.CountryCode))
                return BaseResponse<SignUpCommandResponse>.Fail("Please select valid country phone code.", StatusCodes.Status400BadRequest);

            if (string.IsNullOrWhiteSpace(request.Firstname) || request.Firstname.Length < 2 || request.Firstname.Length > 50)
                return BaseResponse<SignUpCommandResponse>.Fail("Firstname field cannot be null or less than 2 and greater than 50", StatusCodes.Status400BadRequest);

            if (string.IsNullOrWhiteSpace(request.Lastname) || request.Lastname.Length < 2 || request.Lastname.Length > 50)
                return BaseResponse<SignUpCommandResponse>.Fail("Lastname field cannot be null or less than 2 and greater than 50.", StatusCodes.Status400BadRequest);

            #endregion

            var newUser = new CustomerModel
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };

            var userCreationResult = await customerService.CreateCustomerAsync(newUser, request.Password);
            if (!userCreationResult)
                return BaseResponse<SignUpCommandResponse>.Fail("User already exist", StatusCodes.Status409Conflict);

            return BaseResponse<SignUpCommandResponse>.Success(new SignUpCommandResponse(true), StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}