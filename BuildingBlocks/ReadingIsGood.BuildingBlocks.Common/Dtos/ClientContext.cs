namespace ReadingIsGood.BuildingBlocks.Common.Dtos;

public sealed class ClientContext
{
    public string? IpAddress { get; set; }
    public string? Client { get; set; }
    public string? ClientVersion { get; set; }
    public string? Device { get; set; }
    public string? DeviceType { get; set; }
    public string? Os { get; set; }
    public string? OsVersion { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }
    public string? Browser { get; set; }
    public string? BrowserVersion { get; set; }
}
