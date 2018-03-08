using UnityEngine;
using System.Collections;

public class Hex {

    public float a { get; private set; }
    public HexagonOrientation orientation { get; private set; }
    public HexagonOddity oddity { get; private set; }
    public float height { get; private set; }
    public float width { get; private set; }
    public float horiz { get; private set; }
    public float vert { get; private set; }

    public Hex(float a, HexagonOrientation orientation, HexagonOddity oddity) {
        this.a = a;
        this.orientation = orientation;
        this.oddity = oddity;

        if(orientation == HexagonOrientation.FlatTopped) {
            width = a * 2f;
            horiz = width * 3f / 4f;
            height = Mathf.Sqrt(3f) / 2f * width;
            vert = height;
        }
        else {
            height = a * 2f;
            vert = height * 3f / 4f;
            width = Mathf.Sqrt(3f) / 2f * height;
            horiz = height;
        }
    }

    public static OffsetPairs GetNeighborFromDir(Hex hex, CubeDirections dir, OffsetPairs pairs) {

        CubePairs cubePairs = OffsetToCube(hex, pairs.col, pairs.row);

        switch(dir) {
            case CubeDirections.Left:
                if(hex.orientation == HexagonOrientation.PointyTopped)
                    return CubeToOffset(hex, cubePairs.x - 1, cubePairs.y + 1, cubePairs.z);
                break;
            case CubeDirections.Right:
                if(hex.orientation == HexagonOrientation.PointyTopped)
                    return CubeToOffset(hex, cubePairs.x + 1, cubePairs.y - 1, cubePairs.z);
                break;
            case CubeDirections.Top:
                if(hex.orientation == HexagonOrientation.FlatTopped)
                    return CubeToOffset(hex, cubePairs.x, cubePairs.y + 1, cubePairs.z - 1);
                break;
            case CubeDirections.Bottom:
                if(hex.orientation == HexagonOrientation.FlatTopped)
                    return CubeToOffset(hex, cubePairs.x, cubePairs.y - 1, cubePairs.z + 1);
                break;
            case CubeDirections.TopLeft:
                if(hex.orientation == HexagonOrientation.PointyTopped)
                    return CubeToOffset(hex, cubePairs.x, cubePairs.y + 1, cubePairs.z - 1);
                else if(hex.orientation == HexagonOrientation.FlatTopped)
                    return CubeToOffset(hex, cubePairs.x - 1, cubePairs.y + 1, cubePairs.z);
                break;
            case CubeDirections.BottomRight:
                if(hex.orientation == HexagonOrientation.PointyTopped)
                    return CubeToOffset(hex, cubePairs.x, cubePairs.y - 1, cubePairs.z + 1);
                else if(hex.orientation == HexagonOrientation.FlatTopped)
                    return CubeToOffset(hex, cubePairs.x + 1, cubePairs.y - 1, cubePairs.z);
                break;
            case CubeDirections.TopRight:
                return CubeToOffset(hex, cubePairs.x + 1, cubePairs.y, cubePairs.z - 1);
            case CubeDirections.BottomLeft:
                return CubeToOffset(hex, cubePairs.x - 1, cubePairs.y, cubePairs.z + 1);
            default:
                Debug.LogError("Some unknown CubeDirections enum received.");
                return new OffsetPairs(-1, -1);
        }

        return new OffsetPairs(-1, -1);
    }

    public static OffsetPairs CubeToOffset(Hex hex, int x, int y, int z) {

        if(hex == null) {
            Debug.LogError("Hex is null.");
            return new OffsetPairs();
        }

        int row = 0;
        int col = 0;

        int offset;

        if(hex.oddity == HexagonOddity.Odd)
            offset = -1;
        else
            offset = 1;

        // odd-r or even-r
        if(hex.orientation == HexagonOrientation.PointyTopped) {
            col = x + (int)((z + offset * (Mathf.Abs(z) % 2)) / 2);
            row = -z;
        }
        // odd-q or even-q
        else if(hex.orientation == HexagonOrientation.FlatTopped) {
            col = x;
            row = y + (int)((x + offset * (Mathf.Abs(x) % 2)) / 2);
        }

        return new OffsetPairs(row, col);
    }

    public static CubePairs OffsetToCube(Hex hex, int col, int row) {

        if(hex == null) {
            Debug.LogError("Hex is null.");
            return new CubePairs();
        }

        int x = 0;
        int z = 0;
        int y = 0;

        int offset;

        // Vjerojatno zbog naopake mreze
        if(hex.orientation == HexagonOrientation.PointyTopped)
            row = -row;

        if(hex.oddity == HexagonOddity.Even)
            offset = 1;
        else
            offset = -1;

        // odd-r or even-r
        if(hex.orientation == HexagonOrientation.PointyTopped) {
            x = col - (int)((row + offset * (Mathf.Abs(row) % 2)) / 2);
            z = row;
            y = -x - z;
        }
        // odd-q or even-q
        else if(hex.orientation == HexagonOrientation.FlatTopped) {
            x = col;
            y = row - (int)((col + offset * (Mathf.Abs(col) % 2)) / 2);
            z = -x - y;
        }

        return new CubePairs(x, y, z);
    }

    public enum HexagonOrientation {
        FlatTopped, PointyTopped
    }

    public enum HexagonOddity {
        // odd - uvuceno, even - ispupceno
        Odd, Even
    }

    public enum Axis {
        X, Y, Z
    }

    public enum CubeDirections {
        Left, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft
    }

    public struct CubePairs {
        public int x;
        public int y;
        public int z;

        public CubePairs(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct OffsetPairs {
        public int col;
        public int row;

        public OffsetPairs(int col, int row) {
            this.col = col;
            this.row = row;
        }
    }
}
