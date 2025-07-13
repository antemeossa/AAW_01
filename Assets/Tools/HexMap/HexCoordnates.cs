using UnityEngine;
public enum HexDirection
{
    NE = 0,
    E = 1,
    SE = 2,
    SW = 3,
    W = 4,
    NW = 5
}

public struct HexCoordnates
{
    public class HexCoordinates
    {
        public int X { get; private set; }
        public int Z { get; private set; }

        public HexCoordinates(int x, int z)
        {
            X = x;
            Z = z;
        }
    }

}
