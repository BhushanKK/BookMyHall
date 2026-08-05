namespace BookMyHall.Application.Abstractions.Authentication;

public interface IClientInfoService
{
    string? IpAddress { get; }
    string? UserAgent { get; }
    string? Browser { get; }
    string? OperatingSystem { get; }
    string? DeviceType { get; }
    string LoginSource { get; }
}