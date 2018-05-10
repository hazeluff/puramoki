using System.Collections.Generic;
using UnityEngine;

public class MapCoordinate {

    public static readonly MapCoordinate UP = new MapCoordinate(0, 1);
    public static readonly MapCoordinate DOWN = new MapCoordinate(0, -1);
    public static readonly MapCoordinate LEFT = new MapCoordinate(-1, 0);
    public static readonly MapCoordinate RIGHT = new MapCoordinate(1, 0);

    public readonly int X;
    public readonly int Y;
    public Vector3 Vector3 { get { return new Vector3(X, 0, Y); } }

    public MapCoordinate(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static MapCoordinate operator +(MapCoordinate c1, MapCoordinate c2) {
        return new MapCoordinate(c1.X + c2.X, c1.Y + c2.Y);
    }

    public override bool Equals(object obj) {
        var coordinate = obj as MapCoordinate;
        return coordinate != null &&
               X == coordinate.X &&
               Y == coordinate.Y &&
               Vector3.Equals(coordinate.Vector3);
    }

    public override int GetHashCode() {
        var hashCode = -1835336306;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(Vector3);
        return hashCode;
    }
}
