using Common.ConnectionsModels;
using Common.Enums;

namespace Common.Helpers;

public static class ConsoleHelper
{

    public static void ShowRequest(Request request)
    {
        Console.WriteLine();
        ShowProperyHorizantal("Client Name", request.ClientName);
        ShowProperyHorizantal("IpV4 Address", request.ClientIpAddress);
        ShowProperyHorizantal("requested", request.RequestType.ToString(), ConsoleColor.DarkYellow);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\b. ");
        Console.ResetColor();
    }

    public static void ShowRequestCLientInfo(Request request)
    {
        ShowPropery("CLient Name", request.ClientName);
        ShowPropery("IpV4 Address", request.ClientIpAddress);
    }

    public static void ShowStatus(ResultStatus status, string? message = null)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Request Status: ");

        Console.ForegroundColor = GetColorStatusType(status);
        Console.WriteLine(status.ToString());
        Console.ResetColor();

        if (string.IsNullOrEmpty(message))
            return;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{new string(' ', 7)}Request Message: ");
        Console.ForegroundColor = GetColorStatusType(status);
        Console.WriteLine(message);

        Console.ResetColor();

    }

    public static void ShowPropery(string name, string value, ConsoleColor valueColor = ConsoleColor.DarkGreen)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{name}: ");

        Console.ForegroundColor = valueColor;
        Console.WriteLine(value);

        Console.ResetColor();
    }

    public static void ShowProperyHorizantal(string name, string value, ConsoleColor valueColor = ConsoleColor.DarkGreen)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{name}: ");

        Console.ForegroundColor = valueColor;
        Console.Write($"{value} ");

        Console.ResetColor();
    }

    public static void ShowMessage(string text, ResultType type, bool readKey = true)
    {
        Console.ForegroundColor = GetColorFromMessageType(type);
        Console.WriteLine($"{text} ");

        if (readKey)
            Console.ReadKey();

        Console.ResetColor();
    }

    public static ConsoleColor GetColorFromMessageType(ResultType type)
    {
        return type switch
        {
            ResultType.Error => ConsoleColor.DarkRed,
            ResultType.Succes => ConsoleColor.DarkGreen,
            ResultType.Warning => ConsoleColor.DarkYellow,
            _ => ConsoleColor.White,
        };
    }

    public static ConsoleColor GetColorStatusType(ResultStatus type)
    {
        return type switch
        {
            ResultStatus.Succes => ConsoleColor.DarkGreen,
            ResultStatus.Failed => ConsoleColor.DarkRed,
            _ => ConsoleColor.White,
        };
    }

}
