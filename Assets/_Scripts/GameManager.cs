using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static BoundingBox GridBoundingBox;

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
