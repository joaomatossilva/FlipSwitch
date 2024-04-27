using Projects;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql")
    .AddDatabase("flip");
//var sqlServer = builder.add<Flip>("sql");

var backend = builder.AddProject<FlipSwitch_Web>("backend")
    .WithReference(sqlServer);

// builder.AddProject<SimpleConsole>("console")
//     .WithReference(backend);
builder.AddProject<SimpleWebApplication>("web")
    .WithReference(backend);

builder.Build().Run();
