using LinqToTerraServiceProvider.Data;

namespace LinqToTerraServiceProvider.Tests;

[TestFixture]
public class QueryableTerraServceDataTests
{
    [Test]
    public void QueryTest()
    {
        const string prefix = "Lond";
        var client = new TerraServiceClient(prefix);
        var target = new QueryableTerraServiceData<Place>(client);

        var query = from place in target
            where place.Name.StartsWith(prefix)
            select new { place.Name, place.State };

        query.ToArray().Should().NotBeEmpty();
    }
}
