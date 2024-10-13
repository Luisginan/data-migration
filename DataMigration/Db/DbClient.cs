using Npgsql;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public class DbClientPostgres(Config config) : IDbClient
{

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

    private  NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(config.ConnectionString);
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
    
}
