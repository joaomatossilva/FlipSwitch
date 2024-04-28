using FlipSwitch.Web.Api;
using FlipSwitch.Web.Data;
using FlipSwitch.Web.Updater;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<FlipDbContext>("flip");
builder.Services.AddScoped<HubUpdater>();
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapConfigsApi();
app.MapHub<UpdatesHub>("updates"); //should be configurable

app.MapDefaultEndpoints();

app.Run();