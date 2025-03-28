using ItsNotACachingIssue.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer(ApplicationNames.SqlServer);

var database = sqlServer.AddDatabase(ApplicationNames.Database);

var worker = builder
    .AddProject<Projects.ItsNotACachingIssue_MigrationsWorker>(ApplicationNames.Migrations)
    .WithReference(database)
    .WaitFor(database);

builder
    .AddProject<Projects.ItsNotACachingIssue_UserAPI>(ApplicationNames.UserApi)
    .WithReference(database)
    .WaitFor(worker);

builder.Build().Run();
