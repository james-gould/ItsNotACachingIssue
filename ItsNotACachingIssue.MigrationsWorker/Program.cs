using ItsNotACachingIssue.Data;
using ItsNotACachingIssue.MigrationsWorker;
using ItsNotACachingIssue.Shared;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<UserDbContext>(ApplicationNames.Database);

builder.Services.AddOpenTelemetry()
    .WithTracing(x => x.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
