using AutoFixture;
using LinqToTerraServiceProvider.Data;
using LinqToTerraServiceProvider.Service;

namespace LinqToTerraServiceProvider.Tests;

public class TerraServiceClient(string prefix) : ITerraServiceClient, ITerraServiceClientFactory
{
    private readonly Fixture _fixture = new();

    public IEnumerable<Place> GetPlaceList(string location, int numResults, bool mustHaveImage)
    {
        return _fixture.Build<Place>()
            .With(x => x.Name, $"{prefix}{_fixture.Create<string>()}")
            .CreateMany(numResults);
    }

    public ITerraServiceClient Create()
    {
        return this;
    }
}
