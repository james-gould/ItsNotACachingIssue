using ItsNotACachingIssue.Data;
using Microsoft.EntityFrameworkCore;

namespace ItsNotACachingIssue.IntegrationTests
{
    public class UserApiDatabaseUpdateTests(DatabaseTestingFixture fixture) : IClassFixture<DatabaseTestingFixture>
    {
        [Fact]
        public async Task ApiUpdatingOnDifferentContextReflectsHereImmediatelyTest()
        {
            // Arrange.
            if (fixture.UserApiClient is null)
            {
                Assert.Fail($"{nameof(fixture.UserApiClient)} was null when executing test case");
            }

            // Create a DB Context to grab the first user.
            // NOTE: this is the entire DbSet for a single user (cached).
            // Not a projection (not cached).
            var dbContextBeforePutClientCall = new UserDbContext();
            dbContextBeforePutClientCall.Database.SetConnectionString(fixture.DatabaseConnectionString);

            // This unique GUID defines the actual INSTANCE a context, which _includes_ anything cached inside it.
            var dbContextBeforePutClientCallContextId = dbContextBeforePutClientCall.ContextId;

            var user = await dbContextBeforePutClientCall.Users.FirstAsync();
            var firstName = user.FirstName;

            var userId = user.UserId;
            var updatedFirstName = Guid.NewGuid().ToString();

            // Act.
            var response = await fixture.UserApiClient.PutAsync($"users/edit/{userId}/{updatedFirstName}", new StringContent(""));

            // Assert.
            response.EnsureSuccessStatusCode();

            // Our second context, which is a completely different instance.
            var context = new UserDbContext();
            context.Database.SetConnectionString(fixture.DatabaseConnectionString);

            // A user from the first context.
            // NOTE: Cached.
            var updatedUser1 = await dbContextBeforePutClientCall.Users.Where(x => x.UserId == userId).FirstAsync();

            // The same user from the second context.
            // NOTE: Not Cached.
            var updatedUser2 = await context.Users.Where(x => x.UserId == userId).FirstAsync();

            var updatedFirstName1 = updatedUser1.FirstName; // AAA
            var updatedFirstName2 = updatedUser2.FirstName; // BBB

            Assert.NotEqual(context.ContextId, dbContextBeforePutClientCallContextId); // These are different contexts.
            Assert.Equal(firstName, updatedFirstName1); /// AAA == AAA
            Assert.NotEqual(updatedFirstName, updatedFirstName1); // AAA != GUID The first context was cached and didn't see the change from the PUT endpoint context.
            Assert.Equal(updatedFirstName, updatedFirstName2); // GUID == GUID
        }
    }
}
