using ConcurrentApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Concurrency services
builder.Services.AddSingleton<WorkQueue>();
builder.Services.AddSingleton<ProcessingController>();
builder.Services.AddHostedService<BackgroundWorker>();

var app = builder.Build();
Console.WriteLine($"ENV = {builder.Environment.EnvironmentName}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        using var scope = app.Services.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<WorkQueue>();

        for (int i = 0; i < 10; i++)
        {
            await queue.EnqueueAsync(CancellationToken.None);
        }
    });
}

// ---- API endpoints ----
app.MapPost("/enqueue", async (WorkQueue queue, CancellationToken ct) =>
{
    await queue.EnqueueAsync(ct);
    return Results.Accepted();
});

app.MapGet("/stats", (WorkQueue queue) =>
{
    return Results.Ok(queue.GetStats());
});

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapPost("/start", (ProcessingController ctrl) =>
{
    ctrl.Start();
    return Results.Ok("Resumed");
});

app.MapPost("/stop", (ProcessingController ctrl) =>
{
    ctrl.Stop();
    return Results.Ok("Paused");
});

app.Run();
