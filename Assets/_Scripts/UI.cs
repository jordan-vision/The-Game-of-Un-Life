using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI : MonoBehaviour
{
    private int selected = 0;

    private Color red = new Color(1, 0, 0, 0.5f);
    private Color white = new Color(0, 1, 1, 0.5f);

    [SerializeField] private int[] cellsLeft;
    [SerializeField] private Sprite[] cellSprites;
    [SerializeField] private TextMeshProUGUI[] text;

    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject playButton, stopButton, pauseButton, selector, instructions, winScreen, loseScreen, tip;
    [SerializeField] private LevelData levelData;

    public void Select(int id)
    {
        if (!IsPlacementPossible())
        {
            return;
        }

        selected = id == selected ? 0 : id;
        ghost.SetActive(selected != 0);

        if (selected != 0)
        {
            ghost.GetComponent<SpriteRenderer>().sprite = cellSprites[selected - 1];
            ghost.GetComponent<SpriteRenderer>().color = cellsLeft[selected - 1] == 0 ? red : white;
        }
    }

    private void Update()
    {
        if (!IsPlacementPossible())
        {
            return;
        }

        var tilemap = GameManager.Instance.Cells.Tilemap;

        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tilemapPosition = tilemap.WorldToCell(worldPosition);

        var lockedIn = levelData.FreePlay ?
            levelData.GridBoundingBox.Contains(tilemapPosition.x, tilemapPosition.y) :
            levelData.GridBoundingBox.IsOnTheLeftSide(tilemapPosition.x, tilemapPosition.y);

        ghost.transform.position = lockedIn ?
                tilemap.GetCellCenterWorld(tilemapPosition) :
                new(worldPosition.x, worldPosition.y, 0);

        if (!lockedIn)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryRemoveCell(tilemapPosition);
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0) && selected != 0 && cellsLeft[selected - 1] > 0)
        {
            if (GameManager.Instance.Cells.TryAddCell(tilemapPosition, selected) && !levelData.FreePlay)
            {
                cellsLeft[selected - 1]--;
                text[selected - 1].text = cellsLeft[selected - 1].ToString();

                if (cellsLeft[selected - 1] == 0)
                {
                    ghost.GetComponent<SpriteRenderer>().color = red;
                }
            }
        }
    }

    public void Disable()
    {
        selected = 0;
        ghost.SetActive(false);
    }

    public void ShowStopAndPauseButtons()
    {
        playButton.SetActive(false);
        stopButton.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void ShowPlayButton()
    {
        playButton.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void TryRemoveCell(Vector3Int position)
    {
        var removed = GameManager.Instance.Cells.TryRemovecell(position);

        if (removed != 0 && !levelData.FreePlay)
        {
            cellsLeft[removed - 1]++;
            text[removed - 1].text = cellsLeft[removed - 1].ToString();

            if (selected != 0 && cellsLeft[selected - 1] != 0)
            {
                ghost.GetComponent<SpriteRenderer>().color = white;
            }
        }
    }

    public void SwitchToInstructionsScreen()
    {
        if (!winScreen.activeSelf && !loseScreen.activeSelf)
        {
            selector.SetActive(false);
            instructions.SetActive(true);
        }
    }

    public void SwitchToEndScreen(bool win)
    {
        if (levelData.FreePlay)
        {
            return;
        }

        instructions.SetActive(false);

        if (win)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
        }
    }

    public void SwitchToSelection()
    {
        selector.SetActive(true);
        instructions.SetActive(false);
    }

    private bool IsPlacementPossible() => levelData.FreePlay ? !GameManager.Instance.Cells.Running : !GameManager.Instance.Cells.Started;

    public void ShowTip()
    {
        tip.SetActive(true);
    }
}
