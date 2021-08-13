namespace Web.Template.Application.IVectorConnect.Lookups.Factories
{
    using System;

    /// <summary>
    /// GeographyGroup Factory Responsible for Building GeographyGroups
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class GeographyGroupFactory : IDisposable
    {
        ///// <summary>
        ///// Returns a List of GeographyGrouping
        ///// </summary>
        ///// <returns>A List of Geography Groups</returns>
        // public virtual List<GeographyGrouping> Create()
        // {
        // var geographygroups = new List<GeographyGrouping>();
        // List<GeographyGroupingGeography> ivcGeographyGroupGeographies;
        // List<Location> locations;
        // List<Lookups.GeographyGrouping> ivcGeographyGroupings = this.IvcGeographyGroupings(out ivcGeographyGroupGeographies, out locations);

        // foreach (Lookups.GeographyGrouping ivcGeographyGrouping in ivcGeographyGroupings)
        // {
        // geographygroups.Add(this.BuildGeographyGroup(ivcGeographyGroupGeographies, ivcGeographyGrouping, locations));
        // }

        // return geographygroups;
        // }

        ///// <summary>
        ///// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///// </summary>
        // public void Dispose()
        // {
        // GC.Collect();
        // }

        ///// <summary>
        ///// Build up a geography group using the corresponding iVectorConnect Lookups
        ///// </summary>
        ///// <param name="ivcGeographyGroupGeographies">The connect geography group geographies.</param>
        ///// <param name="ivcGeographyGrouping">The connect geography grouping.</param>
        ///// <param name="locations">The locations.</param>
        ///// <returns>A geography group</returns>
        // private GeographyGrouping BuildGeographyGroup(
        // List<GeographyGroupingGeography> ivcGeographyGroupGeographies,
        // Lookups.GeographyGrouping ivcGeographyGrouping,
        // List<Location> locations)
        // {
        // List<Location> listOfLocations = this.GetListOfApplicableLocations(ivcGeographyGroupGeographies, ivcGeographyGrouping, locations);

        // List<Geography> geographies = new List<Geography>();
        // foreach (Location location in listOfLocations)
        // {
        // geographies.Add(this.MapGeographyFromLocation(location));
        // }

        // GeographyGrouping geographyGroup = new GeographyGrouping()
        // {
        // Name = ivcGeographyGrouping.GeographyGroup,
        // Geographies = geographies,
        // Id = ivcGeographyGrouping.GeographyGroupingID
        // };
        // return geographyGroup;
        // }

        ///// <summary>
        ///// Gets a list of locations for the provided geography group
        ///// </summary>
        ///// <param name="ivcGeographyGroupGeographies">The connect geography group geographies.</param>
        ///// <param name="ivcGeographyGrouping">The connect geography grouping.</param>
        ///// <param name="locations">The locations.</param>
        ///// <returns>A List of Locations that are valid for the provided Geography group</returns>
        // private List<Location> GetListOfApplicableLocations(
        // List<GeographyGroupingGeography> ivcGeographyGroupGeographies,
        // Lookups.GeographyGrouping ivcGeographyGrouping,
        // List<Location> locations)
        // {
        // var listOfLocations = new List<Location>();

        // var listOfGeographyGroupGeographies =
        // ivcGeographyGroupGeographies.Where(geo => geo.GeographyGroupingID == ivcGeographyGrouping.GeographyGroupingID)
        // .Select(geo => geo.GeographyID)
        // .ToList();

        // switch (ivcGeographyGrouping.Level.ToLower())
        // {
        // case "region":
        // locations.Where(lo => listOfGeographyGroupGeographies.Contains(lo.GeographyLevel2ID))
        // .GroupBy(lo => lo.GeographyLevel3ID)
        // .Select(group => new { listOfLocations = @group.ToList() });
        // break;
        // case "resort":
        // locations.Where(lo => listOfGeographyGroupGeographies.Contains(lo.GeographyLevel3ID))
        // .GroupBy(lo => lo.GeographyLevel3ID)
        // .Select(group => new { listOfLocations = @group.ToList() });
        // break;
        // }

        // return listOfLocations;
        // }

        ///// <summary>
        ///// Gets lookups from connect that we will need to build up our Geography Group
        ///// </summary>
        ///// <param name="ivcGeographyGroupGeographies">The connect geography group geographies.</param>
        ///// <param name="locations">The locations.</param>
        ///// <returns>A List of geography groups</returns>
        // private List<Lookups.GeographyGrouping> IvcGeographyGroupings(out List<GeographyGroupingGeography> ivcGeographyGroupGeographies, out List<Location> locations)
        // {
        // var geographyGroupLookup = new AsyncLookup("geographyGrouping");
        // List<Lookups.GeographyGrouping> ivcGeographyGroupings =
        // geographyGroupLookup.GetLookup<Lookups.GeographyGrouping>();

        // var geographyGroupGeographiesLookup = new AsyncLookup("GeographyGroupingGeography");
        // ivcGeographyGroupGeographies = geographyGroupGeographiesLookup.GetLookup<GeographyGroupingGeography>();

        // var locationLookup = new AsyncLookup("Location");
        // locations = locationLookup.GetLookup<Location>();

        // return ivcGeographyGroupings;
        // }

        ///// <summary>
        ///// populates a new geography object taking values from a provided location object
        ///// </summary>
        ///// <param name="location">The location.</param>
        ///// <returns>A Geography object populated from a location</returns>
        // private Geography MapGeographyFromLocation(Location location)
        // {
        // var geo = new Geography()
        // {
        // Level3Id = location.GeographyLevel3ID,
        // Level1Id = location.GeographyLevel1ID,
        // Level1Name = location.GeographyLevel1Name,
        // Level2Id = location.GeographyLevel2ID,
        // Level2Name = location.GeographyLevel2Name,
        // Level3Name = location.GeographyLevel3Name
        // };
        // return geo;
        // }
        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}