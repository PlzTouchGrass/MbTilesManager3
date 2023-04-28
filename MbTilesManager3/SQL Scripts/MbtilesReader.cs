using System.Data.SQLite;
namespace MbTilesManager3.SQL_Scripts;
public class MbtilesReader
{
    private readonly string _pathToMbtiles;

    // Constructor to initialize the path to the MBTiles dataset
    public MbtilesReader(string pathToMbtiles)
    {
        this._pathToMbtiles = pathToMbtiles;
    }

    // Function to retrieve a tile from the MBTiles dataset using x, y, and zoom values
    public byte[] GetTile(int x, int y, int zoom)
    {
        try
        {
            // Create a new SQLiteConnection object with the path to the MBTiles dataset
            using (var connection = new SQLiteConnection($"Data Source={_pathToMbtiles}"))
            {
                connection.Open();

                // Prepare the SQL query to retrieve the tile data
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT tile_data FROM tiles WHERE zoom_level = @zoom AND tile_column = @x AND tile_row = @y";
                    command.Parameters.AddWithValue("@zoom", zoom);
                    command.Parameters.AddWithValue("@x", x);
                    command.Parameters.AddWithValue("@y", y);

                    // Execute the query and return the result as a byte array
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (byte[])reader.GetValue(0);
                        }
                        else
                        {
                            throw new Exception("Tile not found.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception and return null
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}