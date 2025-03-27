using UnityEngine;

public class BoundingBox
{
    private int minX, maxX, minY, maxY;

    public int MinX => minX;
    public int MaxX => maxX;
    public int MinY => minY;
    public int MaxY => maxY;

    public BoundingBox(int minX, int maxX, int minY, int maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public bool Contains(int x, int y) => x >= minX && x <= maxX && y >= minY && y <= maxY;
}
