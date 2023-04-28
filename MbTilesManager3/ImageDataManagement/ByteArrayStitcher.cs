namespace MbTilesManager3;

public class ByteArrayStitchedExample
{
    public void Example()
    {
        byte[] topLeft = new byte[] { 1, 2 };
        byte[] topRight = new byte[] { 3, 4 };
        byte[] bottomLeft = new byte[] { 5, 6 };
        byte[] bottomRight = new byte[] { 7, 8 };

        ByteArrayStitcher stitcher = new ByteArrayStitcher(topLeft, topRight, bottomLeft, bottomRight);
        byte[] result = stitcher.StitchArrays();
    }
}

public class ByteArrayStitcher
{
    private readonly byte[] _topLeft;
    private readonly byte[] _topRight;
    private readonly byte[] _bottomLeft;
    private readonly byte[] _bottomRight;

    public ByteArrayStitcher(byte[] topLeft, byte[] topRight, byte[] bottomLeft, byte[] bottomRight)
    {
        _topLeft = topLeft;
        _topRight = topRight;
        _bottomLeft = bottomLeft;
        _bottomRight = bottomRight;
    }

    public byte[] StitchArrays()
    {
        int rowLength = _topLeft.Length + _topRight.Length;
        int numRows = 2;
        byte[] result = new byte[rowLength * numRows];

        // Copy top left array into result
        Array.Copy(_topLeft, 0, result, 0, _topLeft.Length);

        // Copy top right array into result
        Array.Copy(_topRight, 0, result, _topLeft.Length, _topRight.Length);

        // Copy bottom left array into result
        Array.Copy(_bottomLeft, 0, result, rowLength, _bottomLeft.Length);

        // Copy bottom right array into result
        Array.Copy(_bottomRight, 0, result, rowLength + _bottomLeft.Length, _bottomRight.Length);

        return result;
    }
}