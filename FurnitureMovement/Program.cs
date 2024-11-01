using FurnitureMovement.Data;
using FurnitureMovement.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var exampleContextConnectionString = builder.Configuration.GetConnectionString(nameof(OrderContext));
builder.Services.AddDbContext<OrderContext>(options =>
    options.UseNpgsql(exampleContextConnectionString));

builder.Services.AddScoped<IOrderService, OrderService>(); //Строка 1

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var exampleContext = serviceScope.ServiceProvider.GetRequiredService<OrderContext>();
exampleContext?.Database.Migrate();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
