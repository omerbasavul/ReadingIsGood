using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReadingIsGood.Application.Attributes;
using ReadingIsGood.Application.Features.Statistics.Query.GetMonthlyStatistics;
using ReadingIsGood.BuildingBlocks.Common.BaseController;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using ReadingIsGood.Domain.Constants;

namespace ReadingIsGood.Api.Controllers;

/// <summary>
/// Statistics related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[CustomAuthorize]
public class StatisticsController : BaseController
{
    /// <summary>
    /// Get monthly statistics
    /// </summary>
    [CustomAuthorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponse<GetMonthlyStatisticsQueryResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
    [HttpGet("GetMonthlyStatistics/{customerId:guid}")]
    public async ValueTask<IActionResult> GetMonthlyStatistics(Guid customerId) => CreateActionResultInstance(await Mediator.Send(new GetMonthlyStatisticsQueryRequest(customerId)));
}