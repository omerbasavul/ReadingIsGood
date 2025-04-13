using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ReadingIsGood.BuildingBlocks.Common.Wrapper;

public class BaseResponse<T>
{
    public T? Data { get; set; }
    
    public int StatusCode { get; init; }

    public bool IsSuccessful => Errors == null || Errors.Count == 0;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationResult? PaginationData { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; init; } = [];

    #region Helpers

    public static BaseResponse<T> Success(T data, int statusCode, PaginationResult? paginationData = null)
    {
        return new BaseResponse<T> { Data = data, StatusCode = statusCode, PaginationData = paginationData };
    }

    public static BaseResponse<T> Success(int statusCode)
    {
        return new BaseResponse<T> { StatusCode = statusCode };
    }

    public static BaseResponse<T> Fail(int statusCode)
    {
        return new BaseResponse<T> { StatusCode = statusCode };
    }

    public static BaseResponse<T> Fail(List<string> errors, int statusCode)
    {
        return new BaseResponse<T> { Errors = errors, StatusCode = statusCode };
    }

    public static BaseResponse<T> Fail(IEnumerable<IdentityError> errors, int statusCode)
    {
        return new BaseResponse<T> { Errors = errors.Select(x => $"Code: {x.Code}, Error: {x.Description}").ToList(), StatusCode = statusCode };
    }

    public static BaseResponse<T> Fail(string error, int statusCode)
    {
        return new BaseResponse<T> { Errors = [error], StatusCode = statusCode };
    }

    #endregion
}