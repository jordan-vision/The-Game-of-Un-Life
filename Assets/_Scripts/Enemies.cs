using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemies : MonoBehaviour
{
    float timer;
    Tilemap tilemap;

    [SerializeField] float timeBetweenGenerations;
    [SerializeField] TileBase enemyTileBase;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        timer = timeBetweenGenerations;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Generate();
            timer = timeBetweenGenerations;
        }
    }

    private void Generate()
    {
        var toKill = new List<(int, int)> { };
        var toBirth = new List<(int, int)> { };

        for (var x = GameManager.GridBoundingBox.MinX; x <= GameManager.GridBoundingBox.MaxX; x++)
        {
            for (var y = GameManager.GridBoundingBox.MinY; y <= GameManager.GridBoundingBox.MaxY; y++)
            {
                var liveNeighbors = CountLiveNeighbors(x, y);

                // Live cell
                if (tilemap.GetTile(new(x, y)) != null)
                {
                    if (liveNeighbors < 2 || liveNeighbors > 3)
                    {
                        toKill.Add((x, y));
                    } 
                }
                // Dead cell
                else
                {
                    if (liveNeighbors == 3)
                    {
                        toBirth.Add((x, y));
                    }
                }
            }
        }

        foreach (var (x, y) in toKill)
        {
            tilemap.SetTile(new(x, y), null);
        }

        foreach (var (x, y) in toBirth)
        {
            tilemap.SetTile(new(x, y), enemyTileBase);
        }
    }

    private int CountLiveNeighbors(int x, int y)
    {
        var neighbors = new List<(int, int)> { (x+1, y), (x-1, y), (x, y+1), (x, y-1), (x+1, y+1), (x-1, y+1), (x+1, y-1), (x-1, y-1) }
            .Where(xy => GameManager.GridBoundingBox.Contains(xy.Item1, xy.Item2));

        var liveNeighbors = neighbors.Where(xy => tilemap.GetTile(new (xy.Item1, xy.Item2)) == enemyTileBase);

        return liveNeighbors.Count();
    }
}
