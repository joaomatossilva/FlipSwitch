using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Since the dev experience using Aspire is not great, because the SQL container is constantly dropped
// I'm using a container outside Aspire. To use the SQL container on Aspire use instead the commented block

var flip = builder.AddSqlServer("sql1")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("flip");

//var flip = builder.AddConnectionString("flip");

var backend = builder.AddProject<FlipSwitch_Web>("backend")
    .WithReference(flip);

// builder.AddProject<SimpleConsole>("console")
//     .WithReference(backend);

builder.AddProject<SimpleWebApplication>("web")
    .WithReference(backend);

builder.AddProject<FlipSwitch_MigrationsService>("migrationsservice")
    .WaitFor(flip)
    .WithReference(flip);

builder.Build().Run();
