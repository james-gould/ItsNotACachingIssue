using ItsNotACachingIssue.Data;
using Microsoft.EntityFrameworkCore;

namespace ItsNotACachingIssue.IntegrationTests
{
    public class UserApiDatabaseUpdateTests(DatabaseTestingFixture fixture) : IClassFixture<DatabaseTestingFixture>
    {
        [Fact]
        public async Task ApiUpdatingOnDifferentContextReflectsHereImmediatelyTest()
        {
            if (fixture.UserApiClient is null)
                Assert.Fail($"{nameof(fixture.UserApiClient)} was null when executing test case");

            var userId = 1;
            var updatedFirstName = "BBB";

            await fixture.UserApiClient.PutAsync($"users/edit/{userId}/{updatedFirstName}", new StringContent(""));

            var context = new UserDbContext();
            context.Database.SetConnectionString(fixture.DatabaseConnectionString);

            var user = await context.Users.Where(x => x.UserId == userId).FirstAsync();

            Assert.Equal(updatedFirstName, user?.FirstName);
        }
    }
}
