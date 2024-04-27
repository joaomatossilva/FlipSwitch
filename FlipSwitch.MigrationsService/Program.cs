using FlipSwitch.MigrationsService;
using FlipSwitch.Web.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<FlipDbContext>("flip");

var host = builder.Build();
host.Run();
