using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cells : MonoBehaviour
{
    float timer;
    Tilemap tilemap;
    int generation = 0;
    bool running = false;

    [SerializeField] float timeBetweenGenerations;

    public Tilemap Tilemap => tilemap;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        timer = timeBetweenGenerations;
    }

    public void StartRunning()
    {
        running = true;
    }

    private void Update()
    {
        if (!running)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Generate();
            timer = timeBetweenGenerations;
        }
    }

    private void Generate()
    {
        generation++;
        var toKill = new List<(int, int)> { };
        var newEnemies = new List<(int, int)> { };
        var moveBullet = new List<(int, int)> { };

        for (var x = GameManager.GridBoundingBox.MinX; x <= GameManager.GridBoundingBox.MaxX; x++)
        {
            for (var y = GameManager.GridBoundingBox.MinY; y <= GameManager.GridBoundingBox.MaxY; y++)
            {
                var liveNeighbors = CountLiveNeighbors(x, y);
                var tileBase = tilemap.GetTile(new(x, y));

                // Enemy cell
                if (tileBase == GameManager.Instance.EnemyTileBase)
                {
                    if (liveNeighbors < 2 || liveNeighbors > 3)
                    {
                        toKill.Add((x, y));
                    } 
                }

                else
                {
                    if (liveNeighbors == 3)
                    {
                        newEnemies.Add((x, y));
                    }
                }

                if (tileBase == GameManager.Instance.GunnerTileBase)
                {
                    if (generation % 4 == 0) 
                    {
                        moveBullet.Add((x + 1, y));
                    }
                }

                if (tileBase == GameManager.Instance.BulletTileBase)
                {
                    toKill.Add((x, y));
                    if (GameManager.GridBoundingBox.Contains(x + 1, y))
                    {
                        moveBullet.Add((x + 1, y));
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
            tilemap.SetTile(new(x, y), GameManager.Instance.EnemyTileBase);
        }

        foreach (var (x, y) in moveBullet)
        {
            tilemap.SetTile(new(x, y), GameManager.Instance.BulletTileBase);
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
