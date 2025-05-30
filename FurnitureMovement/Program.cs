using FurnitureMovement.Data;
using FurnitureMovement.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise.Bootstrap5;

var builder = WebApplication.CreateBuilder(args);

//��������
builder.Services
    .AddBlazorise(options => options.Immediate = true)
    .AddBootstrap5Providers() 
    .AddFontAwesomeIcons();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var ConnectionString = builder.Configuration.GetConnectionString(nameof(OrderContext));

builder.Services.AddDbContextFactory<OrderContext>(options => options.UseNpgsql(ConnectionString));

builder.Services.AddScoped<IOrderService, OrderService>(); //������ 1
builder.Services.AddScoped<NotificationService>();

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


