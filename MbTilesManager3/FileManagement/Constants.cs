using MbTilesManager3.ImageDataManagement;

namespace MbTilesManager3.FileManagement;
public static class Constants
{
    //INPUT FILE PATH DATA
    public const string InputFilePath = "C:\\Users\\james\\Desktop\\mbtiles\\";
    public const string InputFileName = "test";
    public const string InputFileExtension = ".mbtiles";

    //OUTPUT FILE PATH DATA
    public const string OutputFilePath = "C:\\Users\\james\\Desktop\\mbtiles\\";
    public const string OutputFileName = "test";
    public const string OutputFileExtension = ".mbtiles";

    public const string TableName = "tiles";
    public const MapScheme SelectedMapScheme = MapScheme.Tms;
}

public class FilePathFactory
{
    public readonly InputOutputData InputOutputData;

    public FilePathFactory()
    {
        InputOutputData = new InputOutputData(
            new FilePathData(Constants.InputFilePath,
                Constants.InputFileName, Constants.InputFileExtension),
            new FilePathData(Constants.OutputFilePath,
                Constants.OutputFileName, Constants.OutputFileExtension)
        );
    }
}

public struct InputOutputData
{
    public readonly Dictionary<FilePathType, FilePathData> FilePathData;
    public InputOutputData(FilePathData inputData, FilePathData outputData)
    {
        FilePathData = new Dictionary<FilePathType, FilePathData>();
        FilePathData.Add(FilePathType.Input, inputData);
        FilePathData.Add(FilePathType.Output, outputData);
    }
}

public readonly struct FilePathData
{
    public readonly string FilePath;
    public readonly string FileName;
    public readonly string FileExtension;
    public string FileFullPath => FilePath + FileName + FileExtension;

    public FilePathData(string filePath, string fileName, string fileExtension)
    {
        FilePath = filePath;
        FileName = fileName;
        FileExtension = fileExtension;
    }
}

public enum FilePathType
{
    Error,
    Input,
    Output
}