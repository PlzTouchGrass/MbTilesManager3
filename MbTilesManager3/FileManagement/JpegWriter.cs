using System.Drawing;
using System.Drawing.Imaging;
public class WindowsJpegWriter
{
    private byte[] data;
    private int row;
    private int column;
    private string outputPath;
    private string filePrefix;

    public WindowsJpegWriter(byte[] data, int row, int column, string outputPath, string filePrefix = "")
    {
        this.data = data;
        this.row = row;
        this.column = column;
        this.outputPath = outputPath;
        this.filePrefix = filePrefix;
    }

    public void Save()
    {
        using (var stream = new MemoryStream(data))
        {
            try
            {
                using (var image = Image.FromStream(stream))
                {
                    var fileName = $"{filePrefix}__{row}__{column}.jpeg";
                    var filePath = Path.Combine(outputPath, fileName);
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}