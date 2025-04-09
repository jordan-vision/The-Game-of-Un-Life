using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cells : MonoBehaviour
{
    float timer, currentStepTime;
    int generation = 0;
    bool running = false, started = false, ended = false;

    Tilemap tilemap;
    RandomElementsGenerator randomElementsGenerator;

    [SerializeField] float baseStepTime;
    [SerializeField] LevelData levelData;

    public Tilemap Tilemap => tilemap;

    public bool Running => running;

    public bool Started => started;

    private void Start()
    {
        currentStepTime = baseStepTime;
        timer = currentStepTime;

        tilemap = GetComponent<Tilemap>();
        randomElementsGenerator = GetComponent<RandomElementsGenerator>();

        if (randomElementsGenerator != null)
        {
            randomElementsGenerator.AddWall();
            randomElementsGenerator.AddGunner();
            randomElementsGenerator.AddGunner();
        }

        running = levelData.Autoplay;
        started = running;
        ended = running;
    }

    public void StartRunning()
    {
        running = true;
        started = true;
    }

    private void Update()
    {
        if (!running)
        {
            return;
        }

        if (Input.GetKey(KeyCode.M))
        {
            GameManager.Instance.LoadScene(0);
        }

        currentStepTime = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? baseStepTime / 4 : baseStepTime;

        timer += Time.deltaTime;

        if (timer >= currentStepTime)
        {
            Generate();
            timer = 0;
        }
    }

    private void Generate()
    {
        generation++;
        var toKill = new List<(int, int)> { };
        var newEnemies = new List<(int, int)> { };
        var moveBullet = new List<(int, int)> { };
        var spreadWall = new List<(int, int)> { };

        for (var x = levelData.GridBoundingBox.MinX; x <= levelData.GridBoundingBox.MaxX; x++)
        {
            for (var y = levelData.GridBoundingBox.MinY; y <= levelData.GridBoundingBox.MaxY; y++)
            {
                var liveNeighbors = EnemyNeighbors(x, y).Count;
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
                    if ((generation - 1) % 4 == 0) 
                    {
                        var newCoordinates = levelData.Wraparound ? levelData.GridBoundingBox.Wraparound(x + 1, y) : (x + 1, y);
                        var nextTile = tilemap.GetTile(new(newCoordinates.Item1, newCoordinates.Item2));

                        if (nextTile == GameManager.Instance.EnemyTileBase)
                        {
                            toKill.Add(newCoordinates);
                        } 
                        else if (nextTile == null)
                        {
                            moveBullet.Add(newCoordinates);
                        }
                    }
                }

                if (tileBase == GameManager.Instance.BulletTileBase)
                {
                    toKill.Add((x, y));
                    if (levelData.GridBoundingBox.Contains(x + 1, y))
                    {
                        var newCoordinates = (x + 1, y);
                        var nextTile = tilemap.GetTile(new(newCoordinates.Item1, newCoordinates.Item2));

                        if (nextTile == GameManager.Instance.EnemyTileBase)
                        {
                            toKill.Add(newCoordinates);
                        }
                        else if (nextTile == null)
                        {
                            moveBullet.Add(newCoordinates);
                        }
                    } 
                    else if (levelData.Wraparound)
                    {
                        var newCoordinates = levelData.GridBoundingBox.Wraparound(x + 1, y);
                        var nextTile = tilemap.GetTile(new(newCoordinates.Item1, newCoordinates.Item2));

                        if (nextTile == GameManager.Instance.EnemyTileBase)
                        {
                            toKill.Add(newCoordinates);
                        }
                        else if (nextTile == null)
                        {
                            moveBullet.Add(newCoordinates);
                        }
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
            if (tilemap.GetTile(new(x, y)) == GameManager.Instance.WallTileBase)
            {
                var liveNeighbors = EnemyNeighbors(x, y);
                foreach (var neighbor in liveNeighbors)
                {
                    if (!spreadWall.Contains(neighbor))
                    {
                        spreadWall.Add(neighbor);
                    }
                }
            }

            tilemap.SetTile(new(x, y), GameManager.Instance.EnemyTileBase);
        }

        foreach (var (x, y) in moveBullet)
        {
            tilemap.SetTile(new(x, y), GameManager.Instance.BulletTileBase);
        }

        foreach (var (x, y) in spreadWall)
        {
            tilemap.SetTile(new(x, y), GameManager.Instance.WallTileBase);
        }

        if (!ended)
        {
            var playerCellCount = CountCellsOfTypes(new List<TileBase>() { GameManager.Instance.GunnerTileBase, GameManager.Instance.WallTileBase });
            var enemyCellCount = CountCellsOfTypes(new List<TileBase>() { GameManager.Instance.EnemyTileBase });

            if (enemyCellCount == 0)
            {
                GameManager.Instance.UI.SwitchToEndScreen(true);
                ended = true;
            }
            else if (playerCellCount == 0)
            {
                GameManager.Instance.UI.SwitchToEndScreen(false);
                ended = true;
            }
        }
    }

    private List<(int, int)> EnemyNeighbors(int x, int y)
    {
        var allPotentialNeighbors = new List<(int, int)> { (x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1), (x + 1, y + 1), (x - 1, y + 1), (x + 1, y - 1), (x - 1, y - 1) };
        
        List<(int, int)> actualNeighbors = new();

        if (levelData.Wraparound)
        {
            actualNeighbors = allPotentialNeighbors.Select(xy => levelData.GridBoundingBox.Wraparound(xy.Item1, xy.Item2)).ToList();
        }
        else
        {
            actualNeighbors = allPotentialNeighbors.Where(xy => levelData.GridBoundingBox.Contains(xy.Item1, xy.Item2)).ToList();
        }

        var enemyNeighbors = actualNeighbors.Where(xy => tilemap.GetTile(new (xy.Item1, xy.Item2)) == GameManager.Instance.EnemyTileBase);

        return enemyNeighbors.ToList();
    }

    public bool TryAddCell(Vector3Int position, int cellType)
    {
        if (tilemap.GetTile(position) != null && GameManager.Instance.UI != null)
        {
            GameManager.Instance.UI.TryRemoveCell(position);
        }

        switch (cellType)
        {
            case 1:
                tilemap.SetTile(position, GameManager.Instance.GunnerTileBase);
                break;

            case 2:
                tilemap.SetTile(position, GameManager.Instance.WallTileBase);
                break;

            case 3:
                tilemap.SetTile(position, GameManager.Instance.EnemyTileBase);
                break;

            default:
                break;
        }

        return true;
    }

    public int TryRemovecell(Vector3Int position)
    {
        var tileBase = tilemap.GetTile(position);
        var returnValue = 0;

        if (tileBase == GameManager.Instance.GunnerTileBase)
        {
            returnValue = 1;
        }
        else if (tileBase == GameManager.Instance.WallTileBase)
        {
            returnValue = 2;
        } 
        else if (tileBase == GameManager.Instance.EnemyTileBase)
        {
            returnValue = 3;
        }

        tilemap.SetTile(position, null);

        return returnValue;
    }

    public void Pause()
    {
        running = false;
    }

    private int CountCellsOfTypes(List<TileBase> tileBases)
    {
        var count = 0;

        for (var x = levelData.GridBoundingBox.MinX; x <= levelData.GridBoundingBox.MaxX; x++)
        {
            for (var y = levelData.GridBoundingBox.MinY; y <= levelData.GridBoundingBox.MaxY; y++)
            {
                if (tileBases.Contains(tilemap.GetTile(new (x, y)))) 
                {
                    count++;
                }
            }
        }

        return count;
    }
}
