namespace WolfCache;

public class Program
{
    static void Main(string[] args)
    {
        WolfCacheServer server = WolfCacheServer.GetInstance();

        while (true)
        {
            try
            {
                server.StartServer();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\n{new string('~',30)}Exception\n\n");
                Console.WriteLine(exception);
                Console.WriteLine($"\n\n{new string('~',30)}");
                Console.ResetColor();
            }
        }
    }
}