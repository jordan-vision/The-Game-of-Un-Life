using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static BoundingBox GridBoundingBox;

    public TileBase EnemyTileBase, GunnerTileBase, BulletTileBase;
    public Cells Cells;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        GridBoundingBox = new(-7, 6, -1, 3);
    }

    void Update()
    {
        
    }
}
