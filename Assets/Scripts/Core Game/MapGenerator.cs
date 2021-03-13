using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject Placeholder;
    
    public MapGeneratorBubble[] Bubbles;

    [Range(5, 30)] public int yRows;
}
