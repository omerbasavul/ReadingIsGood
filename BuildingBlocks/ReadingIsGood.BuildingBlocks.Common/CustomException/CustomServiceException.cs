using Microsoft.AspNetCore.Http;

namespace ReadingIsGood.BuildingBlocks.Common.CustomException;

public class CustomServiceException(string message, int? statucCodes = null) : Exception(message)
{
    public int StatucCodes { get; set; } = statucCodes ?? StatusCodes.Status500InternalServerError;
}