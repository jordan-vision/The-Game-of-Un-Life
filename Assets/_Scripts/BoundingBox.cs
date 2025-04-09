using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BoundingBox
{
    [SerializeField] private int minX, maxX, minY, maxY;

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

    public bool IsOnTheLeftSide(int x, int y) => Contains(x, y) && x <= minX + (maxX - minX) / 2;

    public (int, int) Wraparound(int x, int y)
    {
        var actualX = x;
        if (x < minX)
        {
            actualX = maxX;
        }
        if (x > maxX)
        {
            actualX = minX;
        }

        var actualY = y;
        if (y < minY)
        {
            actualY = maxY;
        }
        if (y > maxY)
        {
            actualY = minY;
        }

        return (actualX, actualY);
    }

    public Vector3Int RandomPosition() => new(Random.Range(minX, maxX), Random.Range(minY, maxY));
}
