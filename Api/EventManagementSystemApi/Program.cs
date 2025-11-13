using EventManagementSystemApi.Data;
using EventManagementSystemApi.Extensions;
using EventManagementSystemApi.Middleware;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCustonServices()
    .InjectDbContexts(builder.Configuration)
    .AddCustomIdentity()
    .AddCustomAuthefication(builder.Configuration)
    .AddCustomAuthorization();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
        Console.WriteLine("SeedData успешно выполнен!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при выполнении SeedData: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(configuration =>
{
    configuration.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
