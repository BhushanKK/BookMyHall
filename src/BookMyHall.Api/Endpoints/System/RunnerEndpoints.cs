using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;
using Microsoft.Win32;

namespace BookMyHall.Api.Endpoints.System;

[SupportedOSPlatform("windows")]
public static class RunnerEndpoints
{
    private const string LoggerCategory =
        "BookMyHall.Api.Endpoints.System.RunnerEndpoints";

    private const string DefaultRunnerPattern =
        "actions.runner.*";

    public static void MapRunnerEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/runners")
            .WithTags("Runners");

        /* =========================================================
           GET /api/runners
        ========================================================= */

        group.MapGet("/", (
            IConfiguration configuration,
            ILoggerFactory loggerFactory) =>
        {
            var logger =
                loggerFactory.CreateLogger(LoggerCategory);

            var stopwatch =
                Stopwatch.StartNew();

            logger.LogInformation(
                "Getting GitHub Actions runner services.");

            try
            {
                var runners =
                    GetRunnerServices(
                        configuration,
                        logger);

                stopwatch.Stop();

                logger.LogInformation(
                    "GitHub runner service lookup completed. " +
                    "RunnerCount={RunnerCount}, DurationMs={DurationMs}",
                    runners.Count,
                    stopwatch.ElapsedMilliseconds);

                return Results.Ok(runners);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "Failed to get GitHub Actions runner services.");

                return Results.Problem(
                    title: "Failed to get runner services.",
                    detail: ex.Message,
                    statusCode:
                        StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetRunners")
        .WithSummary("Get GitHub runner services")
        .WithDescription(
            "Returns all GitHub Actions self-hosted runner " +
            "services installed on the Windows machine.")
        .Produces<List<RunnerServiceDto>>(
            StatusCodes.Status200OK)
        .ProducesProblem(
            StatusCodes.Status500InternalServerError);


        /* =========================================================
           GET /api/runners/status
        ========================================================= */

        group.MapGet("/status", (
            IConfiguration configuration,
            ILoggerFactory loggerFactory) =>
        {
            var logger =
                loggerFactory.CreateLogger(LoggerCategory);

            var stopwatch =
                Stopwatch.StartNew();

            logger.LogInformation(
                "Checking GitHub Actions runner service status.");

            try
            {
                var runners =
                    GetRunnerServices(
                        configuration,
                        logger);

                stopwatch.Stop();

                logger.LogInformation(
                    "GitHub runner status check completed. " +
                    "RunnerCount={RunnerCount}, DurationMs={DurationMs}",
                    runners.Count,
                    stopwatch.ElapsedMilliseconds);

                return Results.Ok(runners);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "Failed to check GitHub Actions runner services.");

                return Results.Problem(
                    title: "Failed to get runner status.",
                    detail: ex.Message,
                    statusCode:
                        StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetRunnerStatus")
        .WithSummary("Get runner service status")
        .WithDescription(
            "Lists all GitHub Actions runner services with " +
            "their current status and startup type.")
        .Produces<List<RunnerServiceDto>>(
            StatusCodes.Status200OK)
        .ProducesProblem(
            StatusCodes.Status500InternalServerError);


        /* =========================================================
           POST /api/runners/start-stopped
        ========================================================= */

        group.MapPost("/start-stopped", (
            IConfiguration configuration,
            ILoggerFactory loggerFactory) =>
        {
            var logger =
                loggerFactory.CreateLogger(LoggerCategory);

            return EnsureRunnersRunning(
                configuration,
                logger);
        })
        .WithName("StartStoppedRunners")
        .WithSummary("Start any stopped GitHub runners")
        .WithDescription(
            "Checks all configured GitHub Actions runner " +
            "services and starts any service that is not running.")
        .Produces<RunnerEnsureResponse>(
            StatusCodes.Status200OK)
        .ProducesProblem(
            StatusCodes.Status500InternalServerError);


        /* =========================================================
           POST /api/runners/ensure-running
        ========================================================= */

        group.MapPost("/ensure-running", (
            IConfiguration configuration,
            ILoggerFactory loggerFactory) =>
        {
            var logger =
                loggerFactory.CreateLogger(LoggerCategory);

            return EnsureRunnersRunning(
                configuration,
                logger);
        })
        .WithName("EnsureRunnersRunning")
        .WithSummary("Ensure all GitHub runners are running")
        .WithDescription(
            "Checks all configured GitHub Actions runner " +
            "services and starts every runner that is not running.")
        .Produces<RunnerEnsureResponse>(
            StatusCodes.Status200OK)
        .ProducesProblem(
            StatusCodes.Status500InternalServerError);
    }


    /* =========================================================
       ENSURE RUNNERS RUNNING
    ========================================================= */

    [SupportedOSPlatform("windows")]
    private static IResult EnsureRunnersRunning(
        IConfiguration configuration,
        ILogger logger)
    {
        var stopwatch =
            Stopwatch.StartNew();

        logger.LogInformation(
            "Starting GitHub runner ensure-running operation.");

        try
        {
            /* =====================================================
               WINDOWS CHECK
            ===================================================== */

            if (!OperatingSystem.IsWindows())
            {
                logger.LogWarning(
                    "Runner service management requested on " +
                    "non-Windows operating system.");

                return Results.Ok(
                    new RunnerEnsureResponse
                    {
                        Success = false,

                        Message =
                            "Runner service management is supported only on Windows."
                    });
            }


            /* =====================================================
               GET RUNNERS
            ===================================================== */

            var runners =
                GetRunnerServices(
                    configuration,
                    logger);


            var started =
                new List<string>();

            var alreadyRunning =
                new List<string>();

            var failed =
                new List<string>();


            /* =====================================================
               PROCESS EACH RUNNER
            ===================================================== */

            foreach (var runner in runners)
            {
                logger.LogInformation(
                    "Processing runner service. " +
                    "ServiceName={ServiceName}, " +
                    "Status={Status}, " +
                    "StartType={StartType}",
                    runner.Name,
                    runner.Status,
                    runner.StartType);


                /* =================================================
                   ALREADY RUNNING
                ================================================= */

                if (string.Equals(
                        runner.Status,
                        ServiceControllerStatus.Running.ToString(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    alreadyRunning.Add(
                        runner.Name);

                    logger.LogInformation(
                        "Runner service is already running. " +
                        "ServiceName={ServiceName}",
                        runner.Name);

                    continue;
                }


                /* =================================================
                   DISABLED
                ================================================= */

                if (string.Equals(
                        runner.StartType,
                        "Disabled",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var disabledMessage =
                        $"{runner.Name}: service startup type is Disabled.";

                    failed.Add(
                        disabledMessage);

                    logger.LogWarning(
                        "Runner service is disabled and cannot be started. " +
                        "ServiceName={ServiceName}",
                        runner.Name);

                    continue;
                }


                /* =================================================
                   START SERVICE
                ================================================= */

                try
                {
                    logger.LogInformation(
                        "Attempting to start runner service. " +
                        "ServiceName={ServiceName}",
                        runner.Name);


                    using var service =
                        new ServiceController(
                            runner.Name);


                    service.Refresh();


                    /* =================================================
                       CHECK AGAIN BEFORE START
                    ================================================= */

                    if (service.Status ==
                        ServiceControllerStatus.Running)
                    {
                        alreadyRunning.Add(
                            runner.Name);

                        logger.LogInformation(
                            "Runner became running before Start(). " +
                            "ServiceName={ServiceName}",
                            runner.Name);

                        continue;
                    }


                    /* =================================================
                       START
                    ================================================= */

                    service.Start();


                    logger.LogInformation(
                        "Start command sent successfully. " +
                        "Waiting for service to reach Running state. " +
                        "ServiceName={ServiceName}",
                        runner.Name);


                    /* =================================================
                       WAIT FOR RUNNING
                    ================================================= */

                    service.WaitForStatus(
                        ServiceControllerStatus.Running,
                        TimeSpan.FromSeconds(30));


                    service.Refresh();


                    /* =================================================
                       VERIFY
                    ================================================= */

                    if (service.Status ==
                        ServiceControllerStatus.Running)
                    {
                        started.Add(
                            runner.Name);

                        logger.LogInformation(
                            "Runner service started successfully. " +
                            "ServiceName={ServiceName}",
                            runner.Name);
                    }
                    else
                    {
                        var notRunningMessage =
                            $"{runner.Name}: service did not reach Running state. " +
                            $"CurrentStatus={service.Status}";

                        failed.Add(
                            notRunningMessage);

                        logger.LogError(
                            "Runner service failed to reach Running state. " +
                            "ServiceName={ServiceName}, " +
                            "Status={Status}",
                            runner.Name,
                            service.Status);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    /*
                     * ServiceController.Start() commonly wraps
                     * the Windows Win32Exception inside
                     * InvalidOperationException.
                     */

                    var win32Exception =
                        FindWin32Exception(ex);


                    if (win32Exception is not null)
                    {
                        var errorMessage =
                            $"{runner.Name}: " +
                            $"Windows service operation failed. " +
                            $"Win32Error={win32Exception.NativeErrorCode}, " +
                            $"Message={win32Exception.Message}";

                        failed.Add(
                            errorMessage);


                        if (win32Exception.NativeErrorCode == 5)
                        {
                            logger.LogError(
                                ex,
                                "ACCESS DENIED while starting GitHub runner service. " +
                                "ServiceName={ServiceName}. " +
                                "The IIS application pool identity does not have " +
                                "permission to start this Windows service.",
                                runner.Name);
                        }
                        else
                        {
                            logger.LogError(
                                ex,
                                "Windows service operation failed. " +
                                "ServiceName={ServiceName}, " +
                                "Win32Error={Win32Error}",
                                runner.Name,
                                win32Exception.NativeErrorCode);
                        }
                    }
                    else
                    {
                        var errorMessage =
                            $"{runner.Name}: {ex.Message}";

                        failed.Add(
                            errorMessage);

                        logger.LogError(
                            ex,
                            "Invalid operation while starting runner service. " +
                            "ServiceName={ServiceName}",
                            runner.Name);
                    }
                }
                catch (Win32Exception ex)
                {
                    var errorMessage =
                        $"{runner.Name}: " +
                        $"Windows error {ex.NativeErrorCode} - " +
                        $"{ex.Message}";

                    failed.Add(
                        errorMessage);

                    logger.LogError(
                        ex,
                        "Windows denied runner service operation. " +
                        "ServiceName={ServiceName}, " +
                        "Win32Error={Win32Error}",
                        runner.Name,
                        ex.NativeErrorCode);
                }
                catch (Exception ex)
                {
                    var errorMessage =
                        $"{runner.Name}: {ex.Message}";

                    failed.Add(
                        errorMessage);

                    logger.LogError(
                        ex,
                        "Unexpected error while starting runner service. " +
                        "ServiceName={ServiceName}",
                        runner.Name);
                }
            }


            /* =====================================================
               GET FINAL STATUS
            ===================================================== */

            logger.LogInformation(
                "Getting final GitHub runner service status.");

            var finalRunners =
                GetRunnerServices(
                    configuration,
                    logger);


            stopwatch.Stop();


            /* =====================================================
               RESULT
            ===================================================== */

            var success =
                failed.Count == 0;


            string message;


            if (success)
            {
                if (started.Count > 0)
                {
                    message =
                        $"Runner ensure operation completed successfully. " +
                        $"{started.Count} runner(s) started.";
                }
                else
                {
                    message =
                        "All GitHub runner services are already running.";
                }
            }
            else
            {
                message =
                    "One or more GitHub runner services could not be started. " +
                    $"Started={started.Count}, " +
                    $"AlreadyRunning={alreadyRunning.Count}, " +
                    $"Failed={failed.Count}.";
            }


            logger.LogInformation(
                "GitHub runner ensure-running operation completed. " +
                "Success={Success}, " +
                "Started={StartedCount}, " +
                "AlreadyRunning={AlreadyRunningCount}, " +
                "Failed={FailedCount}, " +
                "DurationMs={DurationMs}",
                success,
                started.Count,
                alreadyRunning.Count,
                failed.Count,
                stopwatch.ElapsedMilliseconds);


            return Results.Ok(new RunnerEnsureResponse
                {
                    Success = success,
                    Message = message,
                    Started = started,
                    AlreadyRunning = alreadyRunning,
                    Failed = failed,
                    Runners = finalRunners
                });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Unexpected error during GitHub runner " +
                "ensure-running operation.");

            return Results.Problem(
                title: "Failed to ensure GitHub runners are running.",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }


    /* =========================================================
       FIND WIN32 EXCEPTION
    ========================================================= */

    private static Win32Exception? FindWin32Exception(
        Exception exception)
    {
        Exception? current =
            exception;


        while (current is not null)
        {
            if (current is Win32Exception win32Exception)
                return win32Exception;

            current = current.InnerException;
        }

        return null;
    }


    /* =========================================================
       GET RUNNER SERVICES
    ========================================================= */

    [SupportedOSPlatform("windows")]
    private static List<RunnerServiceDto> GetRunnerServices(
        IConfiguration configuration,
        ILogger logger)
    {
        /* =====================================================
           WINDOWS CHECK
        ===================================================== */

        if (!OperatingSystem.IsWindows())
        {
            var pattern =
                GetRunnerPattern(
                    configuration);

            var message =
                "GitHub runner service lookup is only supported on Windows. " +
                "The current process is running on a non-Windows host. " +
                $"Configured runner pattern: '{pattern}'. " +
                "Deploy this API on the Windows server that hosts the GitHub Actions runners.";

            logger.LogError(message);
            throw new InvalidOperationException(message);
        }


        /* =====================================================
           GET PATTERN
        ===================================================== */

        var runnerPattern = GetRunnerPattern(configuration);

        logger.LogDebug(
            "Searching for GitHub runner services. " +
            "Pattern={Pattern}",
            runnerPattern);


        /* =====================================================
           GET WINDOWS SERVICES
        ===================================================== */

        var services = ServiceController.GetServices();

        /* =====================================================
           FILTER RUNNER SERVICES
        ===================================================== */

        var matchingServices = services .Where(service => MatchesPattern(service.ServiceName, runnerPattern))
                .OrderBy(service => service.ServiceName, StringComparer.OrdinalIgnoreCase)
                .ToList();

        logger.LogInformation( "Found {RunnerCount} matching GitHub runner services.", matchingServices.Count);

        var result = new List<RunnerServiceDto>(matchingServices.Count);

        /* =====================================================
           MAP SERVICES
        ===================================================== */

        foreach (var service in matchingServices)
        {
            try
            {
                service.Refresh();
                var status = service.Status.ToString();
                var startType = GetServiceStartType(service.ServiceName);
                var dto = new RunnerServiceDto
                {
                    Name = service.ServiceName,
                    Status = status,
                    StartType = startType,
                    DisplayName = service.DisplayName,
                    CanStop = service.CanStop,
                    MachineName = service.MachineName
                };

                result.Add(dto);

                logger.LogDebug(
                    "Runner service found. " +
                    "Name={Name}, " +
                    "Status={Status}, " +
                    "StartType={StartType}",
                    dto.Name,
                    dto.Status,
                    dto.StartType);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to read runner service information. " +
                    "ServiceName={ServiceName}",
                    service.ServiceName);
            }
            finally
            {
                service.Dispose();
            }
        }

        return result;
    }


    /* =========================================================
       GET RUNNER PATTERN
    ========================================================= */

    private static string GetRunnerPattern(
        IConfiguration configuration)
    {
        var configuredPattern = configuration["RunnerService:Pattern"]
            ?? configuration["RunnerService__Pattern"]
            ?? DefaultRunnerPattern;


        if (string.IsNullOrWhiteSpace(configuredPattern))
            return DefaultRunnerPattern;

        return configuredPattern;
    }


    /* =========================================================
       MATCH SERVICE NAME
    ========================================================= */

    private static bool MatchesPattern(
        string serviceName,
        string pattern)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            return false;

        if (string.IsNullOrWhiteSpace(pattern))
            pattern = DefaultRunnerPattern;

        if (pattern.EndsWith("*", StringComparison.Ordinal))
        {
            var prefix = pattern[..^1];
            return serviceName.StartsWith( prefix, StringComparison.OrdinalIgnoreCase);
        }

        return string.Equals(serviceName, pattern, StringComparison.OrdinalIgnoreCase);
    }

    /* =========================================================
       GET SERVICE START TYPE
    ========================================================= */
    [SupportedOSPlatform("windows")]
    private static string GetServiceStartType(
        string serviceName)
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey( $@"SYSTEM\CurrentControlSet\Services\{serviceName}");

            if (key is null)
                return "Unknown";

            var value = key.GetValue("Start");


            if (value is null)
                return "Unknown";

            var startValue = Convert.ToInt32(value);

            return startValue switch
            {
                0 => "Boot",
                1 => "System",
                2 => "Automatic",
                3 => "Manual",
                4 => "Disabled",
                _ => "Unknown"
            };
        }
        catch
        {
            return "Unknown";
        }
    }
}


/* =============================================================
   RUNNER SERVICE DTO
============================================================= */

public sealed class RunnerServiceDto
{
    public string Name { get; set; } =
        string.Empty;

    public string Status { get; set; } =
        string.Empty;

    public string StartType { get; set; } =
        string.Empty;

    public string DisplayName { get; set; } =
        string.Empty;

    public bool CanStop { get; set; }

    public string MachineName { get; set; } =
        string.Empty;
}


/* =============================================================
   RUNNER ENSURE RESPONSE
============================================================= */

public sealed class RunnerEnsureResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } =
        string.Empty;

    public List<string> Started { get; set; } =
        [];

    public List<string> AlreadyRunning { get; set; } =
        [];

    public List<string> Failed { get; set; } =
        [];

    public List<RunnerServiceDto> Runners { get; set; } =
        [];
}