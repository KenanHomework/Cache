using API;
using Common.Models;
using System.Net;
using System.Net.Sockets;

namespace Client;

public class Program
{
    static void Main(string[] args)
    {
        AircraftAPI api = AircraftAPI.GetInstance();
        api.Initialize();
        Aircarft a1 = new Aircarft() { Id = "a1", Serial = "F", Model = "F-22", CallName = "Raptorp", Vendor = "Lockheed Martin" };
        Aircarft a2 = new Aircarft() { Id = "a2", Serial = "SU", Model = "SU-75", CallName = "Checkmate", Vendor = "Sukhoi" };
        Aircarft a3 = new Aircarft() { Id = "a3", Serial = "MIG", Model = "MIG-29", CallName = "Fulcrum", Vendor = "Mikoyan" };

        try
        {
            api.Put(a1);

        }
        catch (Exception)
        {


        }
        Console.WriteLine("a1 put");
        try
        {
        api.Put(a2);

        }
        catch (Exception)
        {


        }
        Console.WriteLine("a2 put");
        api.Put(a3);
        Console.WriteLine("a3 put");
        api.Put(a1);
        Console.WriteLine("a1 put");

        api.Get("a1");
        Console.WriteLine("a1 get");
        api.GetAll();
        Console.WriteLine("get all");

        a2.Vendor = "SUKHOI";

        api.Post(a2);
        Console.WriteLine("a2 post");
    
        api.Get("a1");
        Console.WriteLine("a1 get");
        api.Get("a1");
        Console.WriteLine("a1 get");
        api.Get("a1");
        Console.WriteLine("a1 get");
        api.Get("a1");
        Console.WriteLine("a1 get");
    }
}