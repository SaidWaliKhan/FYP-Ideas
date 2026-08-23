using CrispyKitchen.Application;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Infrastructure;
using CrispyKitchen.WebApi.Middleware;
using CrispyKitchen.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
        policy.WithOrigins("http://localhost:5173") // Vite's default dev port
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();


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

app.Run();