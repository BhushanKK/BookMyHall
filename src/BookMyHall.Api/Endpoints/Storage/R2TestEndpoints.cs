using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Api.Endpoints;

public static class R2TestEndpoints
{
    public static IEndpointRouteBuilder MapR2TestEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/test/r2-upload", async (
            IR2StorageService storageService,
            CancellationToken cancellationToken) =>
        {
            const string objectKey = "test/hello.txt";

            const string content = """
                BookMyHall Cloudflare R2 test.
                """;

            await using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(content));

            await storageService.UploadAsync(
                stream,
                objectKey,
                "text/plain",
                cancellationToken);

            return Results.Ok(new
            {
                message = "File uploaded successfully.",
                objectKey
            });
        })
        .AllowAnonymous();

        return app;
    }
}