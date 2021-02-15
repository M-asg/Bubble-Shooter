using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawnerBody : MonoBehaviour
{
    [SerializeField] BubbleSpawner bubbleSpawner=null;
    private void OnMouseDown()
    {
        if (bubbleSpawner.GetShootingSuperBubble())
        {
            bubbleSpawner.UnSelecctSuperBubble();
        }
        else if(bubbleSpawner.RdyToAction())
        {
            bubbleSpawner.SwapBubble();
        }
    }
}
