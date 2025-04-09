using UnityEngine;

public class RandomElementsGenerator : MonoBehaviour
{
    Cells cells;

    public LevelData LevelData;

    private void Awake()
    {
        cells = GetComponent<Cells>();
    }

    public void AddBomb()
    {
        var bombPosition = LevelData.GridBoundingBox.RandomPosition();
        cells.TryAddCell(bombPosition, 2);
    }

    public void AddGunner()
    {
        var gunnerPosition = LevelData.GridBoundingBox.RandomPosition();
        cells.TryAddCell(gunnerPosition, 1);
    }
}
