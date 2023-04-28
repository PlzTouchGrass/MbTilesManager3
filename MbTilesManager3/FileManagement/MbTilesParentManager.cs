using MbTilesManager3.ImageDataManagement;

public class MbTilesPyramidManager
{
    public readonly MbTile ParentTile;

    public MbTilesPyramidManager(string pathToMbTiles, int zoom, int maxZoom,Tuple<int, int> xyValueParent, MapScheme selectedScheme)
    {
        ParentTile = new MbTile(pathToMbTiles, zoom, maxZoom,xyValueParent, selectedScheme);
    }
    
}