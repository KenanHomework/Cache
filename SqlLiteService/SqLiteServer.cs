using System.Net.Sockets;
using System.Net;
using SqlLiteService.Services;
using Common.Helpers;
using Common.Enums;
using Common.ConnectionsModels;
using System.Text.Json;
using WolfCache.ConnectionsModels;
using Common.Models;

namespace SqlLiteService;

public sealed class SqLiteServer
{

    #region Members

    private static SqLiteServer? _instance = null;

    public static int SQ_LITE_SERVER_PORT = 27001;

    private IPAddress IpAddress = IPAddress.Loopback;

    private TcpClient Client { get; set; }

    private TcpListener Listener { get; set; }

    private NetworkStream Stream { get; set; }

    private BinaryReader BinaryReader { get; set; }

    private BinaryWriter BinaryWriter { get; set; }

    AircraftService Service = AircraftService.GetInstance();

    #endregion

    #region Methods

    public void StartServer()
    {
        Service.CreateTable();

        Listener = new TcpListener(IpAddress, SQ_LITE_SERVER_PORT);
        Listener.Start();

        ConsoleHelper.ShowMessage("SqLite Server Launched.", ResultType.Succes, false);
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

        string id = request.RequestData;

        if (string.IsNullOrWhiteSpace(id))
        {
            SendFailedResponse("Invalid Id!");
            return;
        }

        Aircarft? aircarft = null;

        try
        {
            aircarft = Service.Get(request.RequestData);
        }
        catch (Exception exception)
        {
            SendFailedResponse(exception.Message);
            return;
        }

        if (aircarft is null)
        {
            SendFailedResponse("Aircraft is not found!");
            return;
        }

        SendSuccesResponse(JsonSerializer.Serialize(aircarft));

    }

    public void GetAllRequest()
    {
        List<Aircarft> aircarfts = Service.GetAll();

        if (aircarfts is null)
        {
            SendFailedResponse("DataBase Aircrafts is empty");
            return;
        }

        SendSuccesResponse(JsonSerializer.Serialize(aircarfts));
        ConsoleHelper.ShowStatus(ResultStatus.Succes);
    }

    public void PostRequest(Request request)
    {
        Aircarft? aircarft = JsonSerializer.Deserialize<Aircarft>(request.RequestData);

        if (aircarft is null)
        {
            SendFailedResponse("Aircraft Invalid!");
            return;
        }

        try
        {
            Service.Post(aircarft);
        }
        catch (Exception exception)
        {
            SendFailedResponse(exception.Message);
            return;
        }

        SendSuccesResponse(JsonSerializer.Serialize(aircarft));
    }

    public void PutRequest(Request request)
    {

        Aircarft? aircarft = JsonSerializer.Deserialize<Aircarft>(request.RequestData);

        if (aircarft is null)
        {
            SendFailedResponse("Aircraft Invalid!");
            return;
        }

        try
        {
            Service.Put(aircarft);
        }
        catch (Exception exception)
        {
            SendFailedResponse(exception.Message);
            return;
        }

        SendSuccesResponse(JsonSerializer.Serialize(aircarft));

    }

    public void DeleteRequest(Request request)
    {

        string id = request.RequestData;

        if (string.IsNullOrWhiteSpace(id))
        {
            SendFailedResponse("Invalid Id!");
            return;
        }

        try
        {
            Service.Delete(id);
        }
        catch (Exception exception)
        {
            SendFailedResponse(exception.Message);
            return;
        }

        SendSuccesResponse();

    }



    /* Helper Methods */
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

    void SendSuccesResponse(string responseString)
    {
        ArgumentNullException.ThrowIfNull(responseString, nameof(responseString));

        Response response = new Response()
        {
            ResponseData = responseString,
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

    private SqLiteServer() { }

    public static SqLiteServer GetInstance() => _instance ??= new SqLiteServer();


}
