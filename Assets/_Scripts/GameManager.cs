using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static BoundingBox GridBoundingBox;

    public TileBase EnemyTileBase, GunnerTileBase, BulletTileBase;
    public Cells Cells;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        GridBoundingBox = new(-7, 6, -1, 3);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
