using MbTilesManager3.SQL_Scripts;

namespace MbTilesManager3.ImageDataManagement;

//Dispatch Enum
public enum MapScheme
{
    Error,
    Xyz,
    Tms
}

//Dispatch Enum
public enum SecondaryDirections
{
    Error,
    NorthEast,
    SouthEast,
    NorthWest,
    SouthWest
}

//intermediary struct responible for holding the data for the MbTile class
public struct MbTilesData
{
    public readonly int Zoom;
    public readonly Tuple<int, int> XyValueParent;
    public MbTilesData(int zoom, Tuple<int, int> xyValueParent)
    {
        Zoom = zoom;
        XyValueParent = xyValueParent;
    }
}

//intermediary struct responible for holding the data for the MbTile class as well as the children tiles data
public struct MbTileObj
{
    //Data For The Tile
    public readonly MbTilesData Data;
    
    //Children Tiles Data
    public readonly Dictionary<Tuple<SecondaryDirections, int, int>, byte[]> ChildrenTilesData;
    
    //Constructor
    public MbTileObj(int zoom, Tuple<int, int> xyValueParent, Dictionary<Tuple<SecondaryDirections, int, int>, byte[]> childrenTilesData)
    {
        Data = new MbTilesData(zoom, xyValueParent);
        ChildrenTilesData = childrenTilesData;
    }
}

//Message Struct For MbTile class init cleaner
public struct MbTileMessage
{
    //Data For Tiles
    public readonly string PathToMbTiles;
    public readonly int Zoom;
    public readonly int MaxZoom;
    public readonly Tuple<int, int> XyValueParent;
    public readonly MapScheme SelectedScheme;

    public MbTileMessage(string pathToMbTiles,int zoom,int maxZoom,Tuple<int, int> xyValueParent,MapScheme selectedScheme)
    {
        PathToMbTiles = pathToMbTiles;
        Zoom = zoom;
        MaxZoom = maxZoom;
        XyValueParent = xyValueParent;
        SelectedScheme = selectedScheme;
    }
}

//Container for all data and behaviors for a single tile
public class MbTile
{
    // - Parent Data
    private readonly MbTileObj _parentTile;
    private readonly MbTileMessage _tileMessage;
    private readonly Dictionary<SecondaryDirections, MbTile> _childrenTilesRefs;
    public MbTile(string pathToMbTiles,int currentZoom,int maxZoom,Tuple<int, int> xyValueParent,MapScheme selectedScheme)
    {
        _tileMessage = new MbTileMessage(pathToMbTiles, maxZoom,currentZoom, xyValueParent, selectedScheme);
        
        //Generate Parent Tile
        var factory = new MbTileFactory(pathToMbTiles, currentZoom, xyValueParent, selectedScheme);
        _parentTile = factory.ReturnMbTileMessage();
        
        //Generate Children Tiles
        _childrenTilesRefs = new Dictionary<SecondaryDirections, MbTile>();
        GenerateChildrenTiles(currentZoom, maxZoom,_parentTile.ChildrenTilesData, _tileMessage, _childrenTilesRefs);
    }
    
    // - Generate Children Tiles
    private static void GenerateChildrenTiles(int zoom,int maxZoom,Dictionary<Tuple<SecondaryDirections, int, int>, byte[]> inputDictionary, 
        MbTileMessage tileMessage, Dictionary<SecondaryDirections, MbTile> outputDictionary)
    {
        foreach (var (key, value) in inputDictionary)
        {
            var secondaryDirection = key.Item1;
            var x = key.Item2;
            var y = key.Item3;
            var mbTile = new MbTile(tileMessage.PathToMbTiles, zoom, maxZoom,new Tuple<int, int>(x, y), tileMessage.SelectedScheme);
            outputDictionary.Add(secondaryDirection, mbTile);
        }
    }
    
    // - Merge All Tile Data
    public byte[]? ReturnStitchedTile()
    {
        var zoom = _tileMessage.Zoom;
        var maxZoom = _tileMessage.MaxZoom;
        if (TryToTriggerChildren(zoom,maxZoom,_childrenTilesRefs))
        {
            return OnReturnStitchedTile();
        }
        else
        {
            return null;
        }
    }

    // - On Return Stitched Tile based upon data in the children tiles
    private byte[] OnReturnStitchedTile()
    {
        var topLeft = _parentTile.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.NorthWest, 
            _parentTile.Data.XyValueParent.Item1, _parentTile.Data.XyValueParent.Item2)];
        var topRight = _parentTile.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.NorthEast, 
            _parentTile.Data.XyValueParent.Item1, _parentTile.Data.XyValueParent.Item2)];
        var bottomLeft = _parentTile.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.SouthWest, 
            _parentTile.Data.XyValueParent.Item1, _parentTile.Data.XyValueParent.Item2)];
        var bottomRight = _parentTile.ChildrenTilesData[new Tuple<SecondaryDirections, int, int>(SecondaryDirections.SouthEast, 
            _parentTile.Data.XyValueParent.Item1, _parentTile.Data.XyValueParent.Item2)];
        
        var stitcher = new ByteArrayStitcher(topLeft,topRight,bottomLeft,bottomRight);
        var stitchedTile = stitcher.StitchArrays();
        return stitchedTile;
    }
    
    // - Try To Trigger Children
    // - This is a recursive function that will try to trigger the children tiles to generate their data
    private bool TryToTriggerChildren(int currentZoom,int maxZoom,Dictionary<SecondaryDirections, MbTile> childrenTilesRefrences)
    {
        if (currentZoom == maxZoom)
        {
            return true;
        }
        else
        {
            foreach (var (key, value) in childrenTilesRefrences)
            {
                if (value.TryToTriggerChildren(currentZoom + 1, maxZoom,value._childrenTilesRefs))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

//MbTileFactory responsible for generating the parent tile and the children tiles
public class MbTileFactory
{
    private MbtilesReader _mbtilesReader;
    private readonly string _pathToMbtiles;

    private readonly int _zoom;
    private readonly Tuple<int, int> _xyValueParent;

    private readonly MapScheme _mapScheme;

    public MbTileFactory(string pathToMbtiles, int zoom, Tuple<int, int> xyValueParent, MapScheme mapScheme)
    {
        _pathToMbtiles = pathToMbtiles;
        _zoom = zoom;
        _xyValueParent = xyValueParent;
        _mapScheme = mapScheme;
    }

    //for initial data packaging
    private Tuple<SecondaryDirections, int, int> PackData(SecondaryDirections inputDirections, MapScheme mapScheme,
        int zoom, Tuple<int, int> xyParent)
    {
        var x = xyParent.Item1;
        var y = xyParent.Item2;
        var temp = new MbTileUtility(inputDirections, mapScheme).ReturnCalculation(zoom, x, y);

        var xNext = temp.Item2;
        var yNext = temp.Item3;
        return new Tuple<SecondaryDirections, int, int>(inputDirections, xNext, yNext);
    }

    //for searching for tile
    private KeyValuePair<Tuple<SecondaryDirections, int, int>, byte[]> GetTile(MapScheme mapScheme,
        Tuple<SecondaryDirections, int, int> input, int zoom)
    {
        var key = PackData(input.Item1, mapScheme, zoom, new Tuple<int, int>(input.Item2, input.Item3));
        var value = _mbtilesReader.GetTile(zoom, input.Item2, input.Item3);
        return new KeyValuePair<Tuple<SecondaryDirections, int, int>, byte[]>(key, value);
    }

    //for packing data into the dictionary
    private Dictionary<Tuple<SecondaryDirections, int, int>, byte[]> OnReturnChildrenTiles(MapScheme mapScheme, int updatedZoom, Tuple<int, int> xyParent)
    {
        var northEast = GetTile(mapScheme, PackData(SecondaryDirections.NorthEast, mapScheme, updatedZoom, xyParent),
            updatedZoom);
        var northWest = GetTile(mapScheme, PackData(SecondaryDirections.NorthWest, mapScheme, updatedZoom, xyParent),
            updatedZoom);
        var southEast = GetTile(mapScheme, PackData(SecondaryDirections.SouthEast, mapScheme, updatedZoom, xyParent),
            updatedZoom);
        var southWest = GetTile(mapScheme, PackData(SecondaryDirections.SouthWest, mapScheme, updatedZoom, xyParent),
            updatedZoom);

        return new Dictionary<Tuple<SecondaryDirections, int, int>, byte[]>
        {
            { northEast.Key, northEast.Value },
            { northWest.Key, northWest.Value },
            { southEast.Key, southEast.Value },
            { southWest.Key, southWest.Value }
        };
    }

    //for returning the dictionary
    private Dictionary<Tuple<SecondaryDirections, int, int>, byte[]> ReturnChildrenTiles()
    {
        _mbtilesReader = new MbtilesReader(_pathToMbtiles);
        var updatedZoom = _zoom + 1;
        var updatedXyParent = _xyValueParent;
        return OnReturnChildrenTiles(_mapScheme, updatedZoom, updatedXyParent);
    }

    //for returning the MbTileMessage
    public MbTileObj ReturnMbTileMessage()
    {
        var childrenTiles = ReturnChildrenTiles();
        return new MbTileObj(_zoom, _xyValueParent, childrenTiles);
    }
    // Query the database for the tile, and return the byte array   
}

//MbTileUtility responsible for calculating the x and y values for the children tiles
public class MbTileUtility
{
    private readonly SecondaryDirections _direction;
    private readonly MapScheme _mapScheme;
    public MbTileUtility(SecondaryDirections direction, MapScheme mapScheme)
    {
        _direction = direction;
        _mapScheme = mapScheme;
    }
    //for calculating the next tile
    public Tuple<int, int, int> ReturnCalculation(int zoom, int x, int y)
    {
        return OnReturnCalculation(_direction, zoom, x, y);
    }
    //for calculating the next tile
    private Tuple<int, int, int> OnReturnCalculation(SecondaryDirections direction, int zoom, int x, int y)
    {
        //Y Value converted By Map Scheme
        var convertedY = MapSchemeYDispatch(_mapScheme, y, zoom);

        //Return Calculation
        if (direction == SecondaryDirections.NorthEast)
            return NorthEastReturn(zoom, x, convertedY);
        if (direction == SecondaryDirections.NorthWest)
            return NorthWestReturn(zoom, x, convertedY);
        if (direction == SecondaryDirections.SouthEast)
            return SouthEastReturn(zoom, x, convertedY);
        if (direction == SecondaryDirections.SouthWest)
            return SouthWestReturn(zoom, x, convertedY);
        throw new Exception("Direction not Set");
    }

    //for calculating the next tile
    private Tuple<int, int, int> NorthWestReturn(int zoom, int x, int y)
    {
        var newZoom = zoom + 1;
        var newX = x * 2;
        var newY = y * 2;
        return new Tuple<int, int, int>(newZoom, newX, newY);
    }

    //for calculating the next tile
    private Tuple<int, int, int> SouthWestReturn(int zoom, int x, int y)
    {
        var newZoom = zoom + 1;
        var newX = x * 2;
        var newY = y * 2 + 1;
        return new Tuple<int, int, int>(newZoom, newX, newY);
    }

    //for calculating the next tile
    private Tuple<int, int, int> NorthEastReturn(int zoom, int x, int y)
    {
        var newZoom = zoom + 1;
        var newX = x * 2 + 1;
        var newY = y * 2;
        return new Tuple<int, int, int>(newZoom, newX, newY);
    }

    //for calculating the next tile
    private Tuple<int, int, int> SouthEastReturn(int zoom, int x, int y)
    {
        var newZoom = zoom + 1;
        var newX = x * 2 + 1;
        var newY = y * 2 + 1;
        return new Tuple<int, int, int>(newZoom, newX, newY);
    }

    //for converting the Y value
    private int MapSchemeYDispatch(MapScheme mapScheme, int y, int zoom)
    {
        if (mapScheme == MapScheme.Xyz)
            return y;
        if (mapScheme == MapScheme.Tms)
            return (int)Math.Pow(2, zoom) - y - 1;
        throw new Exception("MapScheme not set");
    }
}