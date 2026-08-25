using CrispyKitchen.Application;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Infrastructure;
using CrispyKitchen.Infrastructure.Persistence;
using CrispyKitchen.WebApi.Middleware;
using CrispyKitchen.WebApi.Services;
using CrispyKitchen.WebApi.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
        policy.WithOrigins("http://localhost:5173") // Vite's default dev port
              .AllowAnyHeader()
              .AllowAnyMethod());
});


// Reads the "Serilog" section above and wires it up as the logging provider
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// One clean summary line per HTTP request — method, path, status, duration.
// Placed first so it wraps and times the ENTIRE pipeline, including
// whatever your exception middleware does further down.
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // must be first — catches everything below it

app.UseHttpsRedirection();
app.UseCors("FrontendDev"); // must come before UseAuthentication/UseAuthorization


app.UseAuthentication(); // WHO are you? (reads the JWT)
app.UseAuthorization();  // are you ALLOWED to do this? (checks the role)

app.MapControllers();
app.MapHub<OrdersHub>("/hubs/orders");

app.Run();
