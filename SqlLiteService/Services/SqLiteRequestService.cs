using Common.ConnectionsModels;
using Common.Enums;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using Common.Models;

namespace SqlLiteService.Services;

public sealed class SqLiteRequestService
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
    public static Request GetCommand(string id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            RequestData = id,
            RequestType = RequestType.GET
        };
    }


    // GetAll Command
    public static Request GetAllCommand() => new() { ClientName = Dns.GetHostName(), ClientIpAddress = GetDeviceIpAddress(), RequestType = RequestType.GET_ALL };


    // Put Command
    public static Request PutCommand(Aircarft aircarft)
    {
        if (aircarft is null) throw new ArgumentNullException(nameof(aircarft));

        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            RequestData = JsonSerializer.Serialize(aircarft),
            RequestType = RequestType.PUT
        };
    }


    // Post Command
    public static Request PostCommand(Aircarft aircarft)
    {
        if (aircarft is null) throw new ArgumentNullException(nameof(aircarft));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            RequestData = JsonSerializer.Serialize(aircarft),
            RequestType = RequestType.POST
        };
    }


    // Delete Command
    public static Request DeleteCommand(string id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));


        return new Request()
        {
            ClientName = Dns.GetHostName(),
            ClientIpAddress = GetDeviceIpAddress(),
            RequestData = id,
            RequestType = RequestType.DELETE
        };
    }

}
