using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Cells cells;

    public static GameManager Instance;
    public TileBase EnemyTileBase, GunnerTileBase, BulletTileBase, WallTileBase;
    public UI UI;

    public Cells Cells => cells;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        cells = FindFirstObjectByType<Cells>();
    }

    private void Update()
    {
        ExitGame();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
