using System.Diagnostics;
using System.Text.Json;

namespace BookMyHall.Api.Endpoints.System;

public static class RunnerEndpoints
{
    public static void MapRunnerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/runners")
            .WithTags("Runners");

        group.MapGet("/", () =>
        {
            var runners = GetRunnerServices();
            return Results.Ok(runners);
        })
        .WithName("GetRunners")
        .WithSummary("Get GitHub runner services")
        .WithDescription("Returns all GitHub Actions self-hosted runner services currently installed on the machine.")
        .Produces<List<RunnerServiceDto>>(StatusCodes.Status200OK);

        group.MapGet("/status", () =>
        {
            var runners = GetRunnerServices();
            return Results.Ok(runners);
        })
        .WithName("GetRunnerStatus")
        .WithSummary("Get runner service status")
        .WithDescription("Lists all GitHub Actions runner services with their current status and startup type.")
        .Produces<List<RunnerServiceDto>>(StatusCodes.Status200OK);

        group.MapPost("/start-stopped", () =>
        {
            if (!OperatingSystem.IsWindows())
            {
                return Results.BadRequest(new { message = "Runner service management is supported only on Windows." });
            }

            var pattern = GetRunnerPattern();
            var output = RunPowerShell(
                $"Get-Service | Where-Object {{ $_.Name -like \"{pattern}\" -and $_.Status -ne 'Running' }} | ForEach-Object {{ Start-Service -Name $_.Name }}; Get-Service | Where-Object {{ $_.Name -like \"{pattern}\" }} | Select-Object Name, Status, StartType | ConvertTo-Json -Compress");

            return Results.Ok(ParseRunnerList(output));
        })
        .WithName("StartStoppedRunners")
        .WithSummary("Start any stopped GitHub runners")
        .WithDescription("Checks all GitHub Actions runner services and starts any service that is currently stopped.")
        .Produces<List<RunnerServiceDto>>(StatusCodes.Status200OK);

    }

    private static List<RunnerServiceDto> GetRunnerServices()
    {
        if (!OperatingSystem.IsWindows())
        {
            return [];
        }

        var pattern = GetRunnerPattern();
        var output = RunPowerShell($"Get-Service | Where-Object {{ $_.Name -like \"{pattern}\" }} | Select-Object Name, Status, StartType | ConvertTo-Json -Compress");
        return ParseRunnerList(output);
    }

    private static string GetRunnerPattern()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        return configuration["RunnerService:Pattern"] ?? "actions.runner.*";
    }

    private static List<RunnerServiceDto> ParseRunnerList(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return [];
        }

        try
        {
            var services = JsonSerializer.Deserialize<List<RunnerServiceDto>>(output, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return services ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static string RunPowerShell(string command)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            return string.Empty;
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return string.IsNullOrWhiteSpace(output) ? error : output;
    }
}

public sealed class RunnerServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StartType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool CanStop { get; set; }
    public string MachineName { get; set; } = string.Empty;
}
