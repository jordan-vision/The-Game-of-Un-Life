using UnityEngine;

public class RandomElementsGenerator : MonoBehaviour
{
    Cells cells;

    public LevelData LevelData;

    private void Awake()
    {
        cells = GetComponent<Cells>();
    }

    public void AddWall()
    {
        var wallPosition = LevelData.GridBoundingBox.RandomPosition();
        cells.TryAddCell(wallPosition, 2);
    }

    public void AddGunner()
    {
        var gunnerPosition = LevelData.GridBoundingBox.RandomPosition();
        cells.TryAddCell(gunnerPosition, 1);
    }
}
