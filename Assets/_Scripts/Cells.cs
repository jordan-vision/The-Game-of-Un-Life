using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cells : MonoBehaviour
{
    float timer;
    Tilemap tilemap;
    int generation = 0;

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
        var newEnemies = new List<(int, int)> { };

        for (var x = GameManager.GridBoundingBox.MinX; x <= GameManager.GridBoundingBox.MaxX; x++)
        {
            for (var y = GameManager.GridBoundingBox.MinY; y <= GameManager.GridBoundingBox.MaxY; y++)
            {
                var liveNeighbors = CountLiveNeighbors(x, y);

                // Live cell
                if (tilemap.GetTile(new(x, y)) == enemyTileBase)
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
                        newEnemies.Add((x, y));
                    }
                }
            }
        }

        foreach (var (x, y) in toKill)
        {
            tilemap.SetTile(new(x, y), null);
        }

        foreach (var (x, y) in newEnemies)
        {
            tilemap.SetTile(new(x, y), enemyTileBase);
        }
    }

    private int CountLiveNeighbors(int x, int y)
    {
        var neighbors = new List<(int, int)> { (x+1, y), (x-1, y), (x, y+1), (x, y-1), (x+1, y+1), (x-1, y+1), (x+1, y-1), (x-1, y-1) }
            .Where(xy => GameManager.GridBoundingBox.Contains(xy.Item1, xy.Item2));

        var liveNeighbors = neighbors.Where(xy => tilemap.GetTile(new (xy.Item1, xy.Item2)) != null);

        return liveNeighbors.Count();
    }
}
