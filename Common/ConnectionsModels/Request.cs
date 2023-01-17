using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Common.Enums;

namespace Common.ConnectionsModels;

public sealed class Request
{

    public RequestType RequestType { get; set; }

    public KeyValuePair<string, string> Pair { get; set; } = new KeyValuePair<string, string>();

    public string RequestData { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string ClientIpAddress { get; set; } = string.Empty;

    public void Send(BinaryWriter BinaryWriter) => BinaryWriter.Write(JsonSerializer.Serialize(this));

}
