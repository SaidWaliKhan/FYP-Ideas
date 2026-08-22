using CrispyKitchen.Application;
using CrispyKitchen.Infrastructure;
using CrispyKitchen.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // must be first — catches everything below it

app.UseHttpsRedirection();

app.UseAuthentication(); // WHO are you? (reads the JWT)
app.UseAuthorization();  // are you ALLOWED to do this? (checks the role)

app.MapControllers();

app.Run();