using LinqToTerraServiceProvider.Data;

namespace LinqToTerraServiceProvider.Service;

public interface ITerraServiceClient
{
    IEnumerable<Place> GetPlaceList(string location, int numResults, bool mustHaveImage);
}
