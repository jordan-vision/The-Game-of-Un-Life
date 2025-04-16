using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Cells cells;

    public static GameManager Instance;
    public TileBase EnemyTileBase, GunnerTileBase, BulletTileBase, BombTileBase, InfiltratorTileBase;
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
        if (Input.GetKey(KeyCode.Escape) || SceneManager.GetActiveScene().buildIndex == 0)
        {
            ExitGame();
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
