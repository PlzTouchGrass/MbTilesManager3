// See https://aka.ms/new-console-template for more information

using MbTilesManager3;
using MbTilesManager3.FileManagement;
using MbTilesManager3.ImageDataManagement;
using MbTilesManager3.SQL_Scripts;

Console.WriteLine("Hello, World!");
int zoomLevel = 12;
string tableName = Constants.TableName;

//CREATE A FILE PATH FACTORY
var filePathFactory = new FilePathFactory();
var inputFilePath = filePathFactory.InputOutputData.FilePathData[FilePathType.Input].FileFullPath;
var outputFilePath = filePathFactory.InputOutputData.FilePathData[FilePathType.Output].FileFullPath;
var mapScheme = Constants.SelectedMapScheme;

//RETURN ALL XY VALUES FOR A GIVEN ZOOM LEVEL
var rowColumnValues = new MbTilesDatabase(inputFilePath,tableName).GetRowColumnValues(zoomLevel);
for(int i = 0; i < rowColumnValues.Length; i++)
{
    //Set Up The X and Y Coordinates
    Console.WriteLine(rowColumnValues[i]);
    var xCoordinate = rowColumnValues[i].Item1;
    var yCoordinate = rowColumnValues[i].Item2;
    
    //CREATE A TILE FACTORY
    var tileCoordinate = new Tuple<int, int>(xCoordinate, yCoordinate);
    var tileFactory = new MbTileFactory(inputFilePath,zoomLevel,tileCoordinate, mapScheme);
    var message = tileFactory.ReturnMbTileMessage();
    
    //STITCH THE 4 CHILD TILES TOGETHER
    var topLeft = message.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.NorthWest, 
        message.Data.XyValueParent.Item1, message.Data.XyValueParent.Item2)];
    var topRight = message.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.NorthEast, 
        message.Data.XyValueParent.Item1, message.Data.XyValueParent.Item2)];
    var bottomLeft = message.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.SouthWest, 
        message.Data.XyValueParent.Item1, message.Data.XyValueParent.Item2)];
    var bottomRight = message.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.SouthEast, 
        message.Data.XyValueParent.Item1, message.Data.XyValueParent.Item2)];
    
    var stitcher = new ByteArrayStitcher(topLeft,topRight,bottomLeft,bottomRight);
    var stitchedTile = stitcher.StitchArrays();
    
    //WRITE THE STITCHED TILE TO THE OUTPUT FILE
    var jpegWriter = new WindowsJpegWriter(stitchedTile, xCoordinate, 
        yCoordinate, outputFilePath, "Tile");
    jpegWriter.Save();
}