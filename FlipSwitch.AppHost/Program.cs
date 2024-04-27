using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var flip = builder.AddSqlServer("sql1").AddDatabase("flip");

//var sqlServer = builder.add<Flip>("sql");

var backend = builder.AddProject<FlipSwitch_Web>("backend")
    .WithReference(flip);

// builder.AddProject<SimpleConsole>("console")
//     .WithReference(backend);
builder.AddProject<SimpleWebApplication>("web")
    .WithReference(backend);

builder.AddProject<FlipSwitch_MigrationsService>("migrationsservice")
    .WithReference(flip);

builder.Build().Run();
