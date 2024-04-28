using FlipSwitch.DependencyInjection;
using FlipSwitch.Memory.DependencyInjection;
using FlipSwitch.SignalR;
using SimpleWebApplication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddConfigurator(opt => opt
    .WithOptions()
    .WithCache()
    .WithHttpBackend("http://backend"));

builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddHostedService<ConfigUpdateService>();

var app = builder.Build();

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

app.UseAuthorization();

app.MapRazorPages();

app.MapDefaultEndpoints();

app.Run();