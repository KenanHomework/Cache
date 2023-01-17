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

        api.Put(a1);
        api.Put(a2);
        api.Put(a3);
        api.Put(a1);
        api.Get("a1");
        api.GetAll();

        a2.Vendor = "SUKHOI";
        api.Post(a2);

        api.Get("a1");
        api.Get("a1");
        api.Get("a1");
        api.Get("a1");

    }
}