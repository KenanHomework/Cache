using Common.Models;

namespace API.Models;

public class RequestedAircraft
{
    public RequestedAircraft(Aircarft aircarft) => Aircarft = aircarft;

    public string? ID => Aircarft.Id;

    public int Count { get; set; } = 0;

    public Aircarft Aircarft { get; set; }

}
