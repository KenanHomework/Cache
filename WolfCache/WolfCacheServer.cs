using Common.ConnectionsModels;
using Common.Enums;
using Common.Helpers;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using WolfCache.ConnectionsModels;

namespace WolfCache;


public sealed class WolfCacheServer
{

    #region Members

    private static WolfCacheServer? _instance;

    public static int WOLF_CACHE_SERVER_PORT = 27002;

    private IPAddress IpAddress = IPAddress.Loopback;

    private TcpClient Client { get; set; }

    private TcpListener Listener { get; set; }

    private NetworkStream Stream { get; set; }

    private BinaryReader BinaryReader { get; set; }

    private BinaryWriter BinaryWriter { get; set; }

    List<KeyValuePair<string, string>> DB = new List<KeyValuePair<string, string>>();

    #endregion

    #region Methods

    public void StartServer()
    {
        Listener = new TcpListener(IpAddress, WOLF_CACHE_SERVER_PORT);
        Listener.Start();

        ConsoleHelper.ShowMessage("Wolf Cache Server Launched.", ResultType.Succes, false);
        Console.WriteLine();
        while (true)
        {
            // Initialize propertyes for listening current session
            Client = Listener.AcceptTcpClient();
            Stream = Client.GetStream();
            BinaryReader = new BinaryReader(Stream);
            BinaryWriter = new BinaryWriter(Stream);

            while (true)
            {
                RecieveCommand();
            }

        }

    }


    public void RecieveCommand()
    {

        string input;
        Request request = new();

        try
        {
            input = BinaryReader.ReadString();
        }
        catch (Exception) { return; }

        if (string.IsNullOrEmpty(input))
            return;

        try
        {
            request = JsonSerializer.Deserialize<Request>(input);

            if (request is null)
                return;
        }
        catch (Exception) { }

        ConsoleHelper.ShowRequest(request);

        switch (request.RequestType)
        {
            case RequestType.GET:
                GetRequest(request);
                break;
            case RequestType.GET_ALL:
                GetAllRequest();
                break;
            case RequestType.POST:
                PostRequest(request);
                break;
            case RequestType.PUT:
                PutRequest(request);
                break;
            case RequestType.DELETE:
                DeleteRequest(request);
                break;
            default:
                ConsoleHelper.ShowMessage("Unknow Command !", ResultType.Warning);
                Console.Clear();
                break;
        }
    }



    /* Command Methods */
    public void GetRequest(Request request)
    {

        if (!ReadPair(request, out KeyValuePair<string, string> pair)) return;

        if (string.IsNullOrWhiteSpace(pair.Key))
        {
            SendFailedResponse("Invalid Key!");
            return;
        }

        if (DBIsEmpty())
            return;

        DB.ForEach(pair =>
        {
            if (pair.Key.Equals(pair.Key))
            {
                SendSuccesResponse(pair);
                return;
            }
        });

        SendFailedResponse("key not found");

    }

    public void GetAllRequest()
    {

        if (DBIsEmpty())
            return;

        BinaryWriter.Write(JsonSerializer.Serialize(DB));
        ConsoleHelper.ShowStatus(ResultStatus.Succes);
    }

    public void PostRequest(Request request)
    {

        if (!ReadPair(request, out KeyValuePair<string, string> pair)) return;

        if (DBIsEmpty())
            return;

        int index = DB.FindIndex(ky => ky.Key.Equals(pair.Key));

        if (index < 0)
        {
            SendFailedResponse("key not found");
            return;
        }

        DB[index] = pair;

        SendSuccesResponse(DB[index]);
    }

    public void PutRequest(Request request)
    {

        if (!ReadPair(request, out KeyValuePair<string, string> pair)) return;

        if (!DBIsEmpty(false) && DB.FindAll(ky => ky.Key.Equals(pair.Key)) is not null)
        {
            SendFailedResponse("key already existed");
            return;
        }

        DB.Add(pair);

        SendSuccesResponse(pair);

    }

    public void DeleteRequest(Request request)
    {

        if (!ReadPair(request, out KeyValuePair<string, string> pair)) return;

        if (DBIsEmpty())
            return;

        int index = DB.FindIndex(kv => kv.Key.Equals(pair.Key));

        if (index < 0)
        {
            SendFailedResponse("key not found");
            return;
        }

        DB.RemoveAt(index);

        SendSuccesResponse();

    }



    /* Helper Methods */
    public bool ReadPair(Request request, out KeyValuePair<string, string> pair)
    {
        pair = new KeyValuePair<string, string>();

        try
        {
            pair = request.Pair;
        }
        catch (Exception exception)
        {
            SendFailedResponse(exception.Message);
            return false;
        }

        return true;
    }

    public bool DBIsEmpty(bool sendFailedResponse = true)
    {
        if (DB.Count <= 0)
        {
            if (sendFailedResponse)
                SendFailedResponse("Cache DataBase is empty");
            return true;
        }

        return false;
    }

    void SendFailedResponse(string message)
    {
        Response response = new Response()
        {
            Result = ResultStatus.Failed,
            Message = message
        };

        BinaryWriter.Write(JsonSerializer.Serialize(response));
        ConsoleHelper.ShowStatus(ResultStatus.Failed, message);
    }

    void SendSuccesResponse(KeyValuePair<string, string> pair)
    {


        Response response = new Response()
        {
            ResponseData = JsonSerializer.Serialize(pair),
            Result = ResultStatus.Succes
        };

        BinaryWriter.Write(JsonSerializer.Serialize(response));
        ConsoleHelper.ShowStatus(ResultStatus.Succes);
    }

    void SendSuccesResponse()
    {
        Response response = new Response()
        {
            Result = ResultStatus.Succes
        };

        BinaryWriter.Write(JsonSerializer.Serialize(response));
        ConsoleHelper.ShowStatus(ResultStatus.Succes);
    }


    #endregion

    private WolfCacheServer() { }


    public static WolfCacheServer GetInstance() => _instance ??= new WolfCacheServer();

}