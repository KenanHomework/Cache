using Common.Models;

namespace API.Models;

public class RequestedAircraftCache
{

    List<RequestedAircraft> Requesteds = new();

    /* Methods */

    public void AddAircraft(Aircarft aircarft)
    {
        ArgumentNullException.ThrowIfNull(aircarft, nameof(aircarft));

        int index = Requesteds.FindIndex(ra => ra.ID.Equals(aircarft.Id));

        // Not found
        if (index == -1)
        {
            Requesteds.Add(new RequestedAircraft(aircarft));
            return;
        }

        Requesteds[index].Count++;
    }

    public List<RequestedAircraft>? GetAndClearReadyForCacheAircrafts()
    {
        var aircrafts = Requesteds.FindAll(ra => ra.Count >= 2);

        Requesteds.RemoveAll(ra => ra.Count >= 2);

        return aircrafts;
    }



}
