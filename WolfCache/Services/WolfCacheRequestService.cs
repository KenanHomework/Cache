using Common.ConnectionsModels;
using Common.Enums;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;

namespace WolfCache.Services;

public sealed class WolfCacheRequestService
{

    private static string GetDeviceIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                // Return Ipv4 Address
                return ip.ToString();
            }
        }
        return IPAddress.Loopback.ToString();
    }

    // Get Command
    public static Request GetCommand(string key)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress= GetDeviceIpAddress(),
            Pair = new KeyValuePair<string, string>(key, string.Empty),
            RequestType = RequestType.GET
        };
    }


    // GetAll Command
    public static Request GetAllCommand() => new() { ClientName = Dns.GetHostName(), ClientIpAddress = GetDeviceIpAddress(), RequestType = RequestType.GET_ALL };


    // Put Command
    public static Request PutCommand(string key, string value)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (value is null) throw new ArgumentNullException(nameof(value));

        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            Pair = new KeyValuePair<string, string>(key, value),
            RequestType = RequestType.PUT
        };
    }


    // Post Command
    public static Request PostCommand(string key, string value)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (value is null) throw new ArgumentNullException(nameof(value));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            Pair = new KeyValuePair<string, string>(key, value),
            RequestType = RequestType.POST
        };
    }


    // Delete Command
    public static Request DeleteCommand(string key)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            Pair = new KeyValuePair<string, string>(key, string.Empty),
            RequestType = RequestType.DELETE
        };
    }

}
