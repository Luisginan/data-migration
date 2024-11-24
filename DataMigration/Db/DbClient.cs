using NawaEncryption;
using Npgsql;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public class DbClientPostgres(Config config) : IDbClient
{
    private static string _connectionString = "";
    public List<HistoryScript> GetHistoryScripts()
    {
        var historyScripts = new List<HistoryScript>();
        using var connection = GetConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT order_number,script_name, script_file_name FROM script_history", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            historyScripts.Add(new HistoryScript
            {
                OrderNumber = reader.GetInt32(reader.GetOrdinal("order_number")),
                ScriptName = reader.GetString(reader.GetOrdinal("script_name")),
                ScriptFileName = reader.GetString(reader.GetOrdinal("script_file_name"))
            });
        }
        return historyScripts;
    }
    
    public List<HistoryScript> GetHistoryScriptsWithContent(string version) 
    {
        var historyScripts = new List<HistoryScript>();
        using var connection = GetConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT order_number,script_name,script_content, script_file_name FROM script_history where script_version = @script_version", connection);
        command.Parameters.AddWithValue("script_version", version);  
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            historyScripts.Add(new HistoryScript
            {
                OrderNumber = reader.GetInt32(reader.GetOrdinal("order_number")),
                ScriptName = reader.GetString(reader.GetOrdinal("script_name")),
                ScriptFileName = reader.GetString(reader.GetOrdinal("script_file_name")),
                ScriptContent = reader.GetString(reader.GetOrdinal("script_content"))
            });
        }
        return historyScripts;
    }

    private  NpgsqlConnection GetConnection()
    {
        if (!string.IsNullOrEmpty(_connectionString))
            return new NpgsqlConnection(_connectionString);
        
        var salt = "33aba0ba-ab2f-42cf-a075-4da60a5b283f";
        var password = config.ConnectionString.Split("Password=")[1].Split(";")[0];
        var passOri = Common.DecryptRijndael(password, salt);
        _connectionString = config.ConnectionString.Replace("Password=" + password, "Password=" + passOri);
        return new NpgsqlConnection(_connectionString);
    }

    public void ExecuteScript(string script)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = new NpgsqlCommand(script, connection);
        command.ExecuteNonQuery();
    }

    public void InsertHistoryScript(ScriptData scriptData)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = new NpgsqlCommand
            ("INSERT INTO script_history (order_number, script_name, script_file_name, script_content, script_version, script_status, script_time) VALUES (@order_number, @script_name, @script_file_name, @script_content, @script_version, @script_status, @script_time)", connection);
        command.Parameters.AddWithValue("order_number", scriptData.OrderNumber);
        command.Parameters.AddWithValue("script_name", scriptData.ScriptName);
        command.Parameters.AddWithValue("script_file_name", scriptData.FullName);
        command.Parameters.AddWithValue("script_content", scriptData.ScriptContent);
        command.Parameters.AddWithValue("script_version", scriptData.Version);
        command.Parameters.AddWithValue("script_status", 1);
        command.Parameters.AddWithValue("script_time", DateTime.Now);
        command.ExecuteNonQuery();
    }

    public List<HistoryScript> FindScript(string file)
    {
        var historyScripts = new List<HistoryScript>();
        using var connection = GetConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT order_number, script_name, script_file_name FROM script_history WHERE script_file_name LIKE '%' || @script_file_name || '%'", connection);
        command.Parameters.AddWithValue("script_file_name", file);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            historyScripts.Add(new HistoryScript
            {
                OrderNumber = reader.GetInt32(reader.GetOrdinal("order_number")),
                ScriptName = reader.GetString(reader.GetOrdinal("script_name")),
                ScriptFileName = reader.GetString(reader.GetOrdinal("script_file_name"))
            });
        }
        return historyScripts;
    }
}
