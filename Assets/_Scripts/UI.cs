using UnityEngine;

public class UI : MonoBehaviour
{
    private int selected = 0;

    [SerializeField] private int[] cellsLeft;
    [SerializeField] private Sprite[] cellSprites;

    [SerializeField] private GameObject ghost;

    public void Select(int id)
    {
        selected = id;

        ghost.SetActive(selected != 0);

        if (selected != 0)
        {
            ghost.GetComponent<SpriteRenderer>().sprite = cellSprites[selected - 1];
        }
    }

    private void Update()
    {
        if (selected == 0)
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
    }
}
