using Common.ConnectionsModels;
using Common.Enums;
using Common.Models;
using SqlLiteService;
using System.Net.Sockets;
using System.Net;
using WolfCache;
using System.Text.Json;
using WolfCache.ConnectionsModels;
using Common.Helpers;
using API.Models;
using System.Collections.Immutable;
using SqlLiteService.Services;
using WolfCache.Services;
using System.IO;

namespace API;

public sealed class AircraftAPI
{

    #region Members

    // Singleton instance
    private static AircraftAPI? _instance;

    /* Port Numbers */
    int WOLF_CACHE_SERVER_PORT => WolfCacheServer.WOLF_CACHE_SERVER_PORT;

    int SQ_LITE_SERVER_PORT => SqLiteServer.SQ_LITE_SERVER_PORT;

    /* Common Client Instances */
    public static IPAddress IpAddress = IPAddress.Loopback;

    /* WolfCache TCP Client Instance */
    public static TcpClient WolfCacheTcpClient { get; set; } = new TcpClient();
    public static NetworkStream WolfCacheStream { get; set; }
    public static BinaryReader WolfCacheBinaryReader { get; set; }
    public static BinaryWriter WolfCacheBinaryWriter { get; set; }

    /* SqLite TCP Client Instance */
    public static TcpClient SqLiteTcpClient { get; set; } = new TcpClient();
    public static NetworkStream SqLiteStream { get; set; }
    public static BinaryReader SqLiteBinaryReader { get; set; }
    public static BinaryWriter SqLiteBinaryWriter { get; set; }

    RequestedAircraftCache RequestedAircraftCache = new();

    #endregion

    #region Singleton

    private AircraftAPI() { }

    public static AircraftAPI GetInstance() => _instance ??= new AircraftAPI();

    public void Initialize()
    {
        /* Initalize WolfCache TCP Client Instances */
        WolfCacheTcpClient = new TcpClient();
        WolfCacheTcpClient.Connect(IpAddress, WOLF_CACHE_SERVER_PORT);
        WolfCacheStream = WolfCacheTcpClient.GetStream();
        WolfCacheBinaryWriter = new BinaryWriter(WolfCacheStream);
        WolfCacheBinaryReader = new BinaryReader(WolfCacheStream);

        /* Initalize SqLite TCP Client Instances */
        SqLiteTcpClient = new TcpClient();
        SqLiteTcpClient.Connect(IpAddress, SQ_LITE_SERVER_PORT);
        SqLiteStream = SqLiteTcpClient.GetStream();
        SqLiteBinaryWriter = new BinaryWriter(SqLiteStream);
        SqLiteBinaryReader = new BinaryReader(SqLiteStream);
    }

    #endregion

    #region Command Methods

    public Response Get(string id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        Response? response;

        response = SendRequestToWolfCache(WolfCacheRequestService.GetCommand(id));


        if (response is not null
            && response.Result == ResultStatus.Succes)
        {
            AddAircraftAndSyncCache(response.ResponseData);
            return response;
        }

        response = SendRequestToSqLiteServer(SqLiteRequestService.GetCommand(id));
        AddAircraftAndSyncCache(response.ResponseData);
        return response;
    }

    public Response GetAll() => SendRequestToSqLiteServer(SqLiteRequestService.GetAllCommand());

    public Response Put(Aircarft aircarft)
    {
        ArgumentNullException.ThrowIfNull(aircarft, nameof(aircarft));

        return SendRequestToSqLiteServer(SqLiteRequestService.PutCommand(aircarft));
    }

    public Response Post(Aircarft aircarft)
    {
        ArgumentNullException.ThrowIfNull(aircarft, nameof(aircarft));

        Response response;

        response = SendRequestToWolfCache(WolfCacheRequestService.PostCommand(aircarft.Id, JsonSerializer.Serialize(aircarft)));

        if (response.Result == ResultStatus.Succes) return response;

        return SendRequestToSqLiteServer(SqLiteRequestService.PostCommand(aircarft));
    }

    public Response Delete(string id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        Response response;

        response = SendRequestToWolfCache(WolfCacheRequestService.DeleteCommand(id));

        if (response.Result == ResultStatus.Succes) return response;

        return SendRequestToSqLiteServer(SqLiteRequestService.DeleteCommand(id));
    }

    #endregion

    #region Helper Methods
    /* Connection Servers */
    public Response SendRequestToWolfCache(Request request) => SendRequestToServer(request, WolfCacheBinaryWriter, WolfCacheBinaryReader);

    public Response SendRequestToSqLiteServer(Request request) => SendRequestToServer(request, SqLiteBinaryWriter, SqLiteBinaryReader);

    public static Response SendRequestToServer(Request request, BinaryWriter binaryWriter, BinaryReader binaryReader)
    {
        ArgumentNullException.ThrowIfNull(nameof(request));
        try
        {

            binaryWriter.Write(JsonSerializer.Serialize(request));

            string responseString = binaryReader.ReadString();


            if (string.IsNullOrWhiteSpace(responseString))
                return new Response() { Result = ResultStatus.Failed };


#pragma warning disable CS8603 // Possible null reference return.
            return JsonSerializer.Deserialize<Response>(responseString);
#pragma warning restore CS8603 // Possible null reference return.
        }
        catch (Exception)
        {
            return new Response() { Result = ResultStatus.Failed };
        }
    }

    /* Requesteds */

    void AddAircraftAndSyncCache(string? aircarftString)
    {
        Aircarft? aircarft = null;
        try
        {
            aircarft = JsonSerializer.Deserialize<Aircarft>(aircarftString);
        }
        catch (Exception) { }

        if (aircarft is not null) RequestedAircraftCache.AddAircraft(aircarft);

        var aircrafts = RequestedAircraftCache.GetAndClearReadyForCacheAircrafts();

        if (aircrafts is null) return;

        aircrafts.ForEach(a =>
        {
            SendRequestToWolfCache(WolfCacheRequestService.PutCommand(a.ID, JsonSerializer.Serialize(a.Aircarft)));
        });

    }

    #endregion

}
