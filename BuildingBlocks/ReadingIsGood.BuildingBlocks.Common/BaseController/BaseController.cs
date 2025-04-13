using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ReadingIsGood.BuildingBlocks.Common.Dtos;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using UAParser;

namespace ReadingIsGood.BuildingBlocks.Common.BaseController;

[ApiController, Route("api/[controller]")]
public class BaseController : ControllerBase
{
    private IMediator? _mediator;
    private Guid? _userId;
    private ClientContext? _clientInfoContext;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

    protected Guid UserId => _userId ??= HttpContext.Items.TryGetValue("UserId", out var value) ? Guid.Parse(value?.ToString() ?? string.Empty) : Guid.Empty;

    protected static IActionResult CreateActionResultInstance<T>(BaseResponse<T> response) => new ObjectResult(response) { StatusCode = response.StatusCode };

    protected ClientContext ClientInfoContext => _clientInfoContext ??= CreateClientContext();

    private ClientContext CreateClientContext()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var parser = Parser.GetDefault();
        var clientInfo = parser.Parse(userAgent);

        return new ClientContext
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),

            Device = clientInfo.Device.Family ?? "Unknown",
            DeviceType = DetectDeviceType(clientInfo),

            Os = clientInfo.OS.Family ?? "Unknown",
            OsVersion = GetOsVersion(clientInfo.OS),

            Browser = clientInfo.UA.Family ?? "Unknown",
            BrowserVersion = GetBrowserVersion(clientInfo.UA),

            UserAgent = userAgent,
            Referer = HttpContext.Request.Headers.Referer.ToString()
        };
    }


    private static string GetOsVersion(OS os)
    {
        var version = $"{os.Major}.{os.Minor}".TrimEnd('.');
        return string.IsNullOrWhiteSpace(version) ? "Unknown" : version;
    }

    private static string GetBrowserVersion(UserAgent ua)
    {
        var version = $"{ua.Major}.{ua.Minor}".TrimEnd('.');
        return string.IsNullOrWhiteSpace(version) ? "Unknown" : version;
    }

    private static string DetectDeviceType(ClientInfo client)
    {
        var device = client.Device.Family?.ToLower() ?? "";

        if (device.Contains("mobile") || device.Contains("iphone") || device.Contains("android"))
            return "Mobile";

        if (device.Contains("tablet") || device.Contains("ipad"))
            return "Tablet";

        if (device.Contains("pc") || device.Contains("mac") || device.Contains("windows"))
            return "Desktop";

        return "Unknown";
    }
}