using LinqToTerraServiceProvider.Data;
using LinqToTerraServiceProvider.Service;

namespace LinqToTerraServiceProvider.Internal;

internal class DataServiceHelper(ITerraServiceClientFactory clientFactory)
{
    private const int NumResults = 200;
    private const bool MustHaveImage = false;

    public IEnumerable<Place> GetPlacesFromTerraService(List<string> locations)
    {
        if (locations.Count > 5)
        {
            throw new InvalidQueryException("This query requires more than five separate calls to the Web service. Please decrease the number of locations in your query.");
        }

        var allPlaces = new List<Place>();

        var client = clientFactory.Create();
        foreach (var location in locations)
        {
            var places = client.GetPlaceList(location, NumResults, MustHaveImage);
            allPlaces.AddRange(places);
        }

        return allPlaces.ToArray();
    }
}
