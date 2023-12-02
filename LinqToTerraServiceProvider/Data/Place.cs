namespace LinqToTerraServiceProvider.Data;

// ReSharper disable NotAccessedPositionalProperty.Global
public record Place(string Name, string State, PlaceType PlaceType);
// ReSharper restore NotAccessedPositionalProperty.Global

public enum PlaceType
{
    Unknown,
    AirRailStation,
    BayGulf,
    CapePeninsula,
    CityTown,
    HillMountain,
    Island,
    Lake,
    OtherLandFeature,
    OtherWaterFeature,
    ParkBeach,
    PointOfInterest,
    River
}
