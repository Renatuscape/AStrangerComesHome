using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static void Initialise(Location location)
    {
        objectIDReader(ref location);
    }

    public static void objectIDReader(ref Location location)
    {

        //SET REGION
        if (int.TryParse(location.objectID.Substring(1, 1), out var i))
        {
            var region = Regions.FindByID("REGION"+i);
            if (region is null)
            {
                Debug.Log("Could not find REGION"+i);
            }
            region.locations.Add(location);
        }

        //SET TYPE
        string type = location.objectID.Substring(9, 4);
        if (type == "CITY")
        {
            location.type = LocationType.City;
        }
        else if (type == "TOWN")
        {
            location.type = LocationType.Town;
        }
        else if (type == "SETL")
        {
            location.type = LocationType.Settlement;
        }
        else if (type == "OUTP")
        {
            location.type = LocationType.Outpost;
        }
        else if (type == "STOP")
        {
            location.type = LocationType.Stop;
        }
        else if (type == "CROS")
        {
            location.type = LocationType.Crossing;
        }
        else if (type == "HIDD")
        {
            location.type = LocationType.Hidden;
        }
        else
        {
            Debug.Log($"Location tag ({type}) not recognised at {location.objectID}");
        }
    }
}
