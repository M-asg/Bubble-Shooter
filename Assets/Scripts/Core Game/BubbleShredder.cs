using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        KinematicBubble kinematicBubble = other.GetComponent<KinematicBubble>();
        if (kinematicBubble)
        {
            StartCoroutine(kinematicBubble.DestroyBubble());
        }
    }
}
