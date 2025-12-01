var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres")
    .WithPgAdmin()
    .WithDataVolume();

var todoDb = postgres.AddDatabase("todo");

var redis = builder.AddRedis("Redis")
    .WithRedisInsight()
    .WithDataVolume();

var rabbitMq = builder.AddRabbitMQ("RabbitMq")
    .WithManagementPlugin()
    .WithDataVolume();

var apiService = builder.AddProject<Projects.Ais_ToDo_Api>("api-service")
    .WaitFor(postgres)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WithReference(todoDb, connectionName: "Postgres")
    .WithReference(redis)
    .WithReference(rabbitMq);

builder.Build().Run();