using Common.Models;
using SqlLiteService.Services;

namespace SqlLiteService;

public class Program
{
    static void Main(string[] args)
    {
        SqLiteServer server = SqLiteServer.GetInstance();

        while (true)
        {
            try
            {
                server.StartServer();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\n{new string('~', 30)}Exception\n\n");
                Console.WriteLine(exception);
                Console.WriteLine($"\n\n{new string('~', 30)}");
                Console.ResetColor();
            }
        }

    }
}