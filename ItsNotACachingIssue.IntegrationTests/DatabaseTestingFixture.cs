
using Aspire.Hosting;
using ItsNotACachingIssue.Shared;

namespace ItsNotACachingIssue.IntegrationTests
{
    public class DatabaseTestingFixture : IAsyncLifetime
    {
        internal readonly TimeSpan _waitPeriod = TimeSpan.FromMinutes(2);
        internal DistributedApplication? _app;
        internal ResourceNotificationService? _notificationService;

        public HttpClient? UserApiClient;
        public string DatabaseConnectionString = string.Empty;

        public async Task InitializeAsync()
        {
            var builder = await DistributedApplicationTestingBuilder
                            .CreateAsync<Projects.ItsNotACachingIssue_AppHost>([], (x, y) => x.DisableDashboard = true);

            _app = await builder.BuildAsync();

            await _app.StartAsync();

            _notificationService = _app.Services.GetService<ResourceNotificationService>();

            await WaitForResourceAsync(ApplicationNames.UserApi);

            var userApiEndpoint = _app.GetEndpoint(ApplicationNames.UserApi);

            UserApiClient = new HttpClient
            {
                BaseAddress = userApiEndpoint
            };

            DatabaseConnectionString = await _app.GetConnectionStringAsync(ApplicationNames.Database) 
                                                ?? throw new InvalidOperationException($"Failed to find DbConnection for {ApplicationNames.Database}");
        }

        public async Task DisposeAsync()
        {
            if (_app is not null)
                await _app.DisposeAsync();

            if(UserApiClient is not null)
                UserApiClient.Dispose();
        }

        private async Task WaitForResourceAsync(string applicationName)
        {
            if (_notificationService is null)
                throw new Exception("Notif null");

            await _notificationService.WaitForResourceAsync(applicationName, KnownResourceStates.Running).WaitAsync(_waitPeriod);
        }
    }
}
