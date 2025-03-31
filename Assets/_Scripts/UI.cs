using TMPro;
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
    [SerializeField] private GameObject stopButton;

    public void Select(int id)
    {
        if (GameManager.Instance.Cells.Running)
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
        if (GameManager.Instance.Cells.Running)
        {
            return;
        }

        var tilemap = GameManager.Instance.Cells.Tilemap;

        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tilemapPosition = tilemap.WorldToCell(worldPosition);

        var lockedIn = (GameManager.GridBoundingBox.IsOnTheLeftSide(tilemapPosition.x, tilemapPosition.y));

        ghost.transform.position = lockedIn ?
                tilemap.GetCellCenterWorld(tilemapPosition) :
                new(worldPosition.x, worldPosition.y, 0);

        if (!lockedIn)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var removed = GameManager.Instance.Cells.TryRemovecell(tilemapPosition);

            if (removed != 0)
            {
                cellsLeft[removed - 1]++;
                text[removed - 1].text = cellsLeft[removed - 1].ToString();

                if (selected != 0 && cellsLeft[selected - 1] != 0)
                {
                    ghost.GetComponent<SpriteRenderer>().color = white;
                }
            }
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0) && selected != 0 && cellsLeft[selected - 1] > 0)
        {
            if (GameManager.Instance.Cells.TryAddCell(tilemapPosition, selected))
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

    public void ShowStopButton()
    {
        stopButton.SetActive(true);
    }
}
