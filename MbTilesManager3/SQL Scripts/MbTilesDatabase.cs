using System.Data.SQLite;

namespace MbTilesManager3.SQL_Scripts;

public class MbTilesDatabaseExample
{
    public void Example()
    {
        MbTilesDatabase mbTilesDb = new MbTilesDatabase("path/to/your/database.db", "your_table_name");
        (int, int)[] rowColumnValues = mbTilesDb.GetRowColumnValues(10);
    }
}

public class MbTilesDatabase
{
    private readonly string _dbPath;
    private readonly string _tableName;

    public MbTilesDatabase(string dbPath, string tableName)
    {
        this._dbPath = dbPath;
        this._tableName = tableName;
    }

    public (int, int)[] GetRowColumnValues(int zoomLevel)
    {
        // create the SQL query to retrieve the row and column values
        string query = $"SELECT tile_row, tile_column FROM {_tableName} WHERE zoom_level = {zoomLevel}";

        // create a list to store the tuples of row and column values
        List<(int, int)> rowColumnValues = new List<(int, int)>();

        // create a connection to the database and execute the query
        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // read each row returned by the query and add the row and column values to the list
                    while (reader.Read())
                    {
                        int row = reader.GetInt32(0);
                        int column = reader.GetInt32(1);
                        rowColumnValues.Add((row, column));
                    }
                }
            }
        }

        // convert the list of tuples to an array and return it
        return rowColumnValues.ToArray();
    }
}