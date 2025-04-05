using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pg = builder.AddPostgres("pg")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var backend = builder.AddProject<FlipSwitch_Web>("backend")
    .WithReference(pg);

builder.AddProject<SimpleWebApplication>("web")
    .WithReference(backend);

builder.Build().Run();
