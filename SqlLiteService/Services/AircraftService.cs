using Common.Models;
using Dapper;
using System.Data;
using System.Data.SQLite;

namespace SqlLiteService.Services;

public sealed class AircraftService
{

    private AircraftService() { }

    private static AircraftService? _instance = null;

    private IDbConnection DB { get; set; } = new SQLiteConnection("Data Source = DB.sqlite3;");

    public static AircraftService GetInstance() => _instance ??= new AircraftService();

    public void OpenDataBase() => DB.Open();

    public Aircarft? Get(string id)
    {

        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

        string query = @"SELECT
                        	*
                        FROM
                        	Aircrafts
                        WHERE
                        	Id = @Id";

        try
        {
            return DB.Query<Aircarft>(query, new { Id = id }).ToList().FirstOrDefault();
        }
        catch (Exception) { throw; }

    }

    public List<Aircarft> GetAll()
    {
        string query = @"
                        SELECT
                        	*
                        FROM
                        	Aircrafts";

        try
        {
            return DB.Query<Aircarft>(query).ToList();
        }
        catch (Exception) { throw; }

    }

    public void Put(Aircarft aircarft)
    {
        var query = @"
                insert into Aircrafts (Id, Vendor, Serial, Model, CallName)
                VALUES
                	(@Id, @Vendor, @Serial, @Model, @CallName)";

        try
        {
            DB.Execute(query, aircarft);
        }
        catch (Exception) { throw; }

    }

    public void Post(Aircarft aircarft)
    {
        string query = @"UPDATE
                        	Aircrafts
                        SET
                        	Vendor = @Vendor,
                        	Serial = @Serial,
                        	Model = @Model,
                        	CallName = @CallName
                        WHERE
                        	Id = @Id";

        try
        {
            DB.Execute(query, aircarft);
        }
        catch (Exception) { throw; }

    }

    public void Delete(string id)
    {

        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

        var query = @"
                    DELETE FROM
                    	Aircrafts
                    WHERE
                    	Id = @Id";
        try
        {
            DB.Execute(query, new { Id = id });
        }
        catch (Exception) { throw; }

    }

    public void CreateTable()
    {
        try
        {
            DB.Execute(@"CREATE TABLE Aircrafts(
	                            Id TEXT PRIMARY KEY UNIQUE NOT NULL,
	                            Vendor   TEXT  NOT NULL,
	                            Serial   TEXT  NOT NULL,
	                            Model    TEXT  NOT NULL,
	                            CallName TEXT  NOT NULL
                                )");
        }
        catch { }
    }

}
