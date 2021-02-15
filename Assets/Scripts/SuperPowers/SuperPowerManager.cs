using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPowerManager : MonoBehaviour
{
    //config
    [SerializeField] SuperPower fireBubble = null;
    [SerializeField] SuperPower thunderBubble = null;


    //cache
    BubbleSpawner bubbleSpawner = null;

    private void Start()
    {
        bubbleSpawner = FindObjectOfType<BubbleSpawner>();
    }

    public void SetFireBubble()
    {
        bubbleSpawner.SelectSuperBubble(fireBubble);
    }

    public void SetTunderBubble()
    {
        bubbleSpawner.SelectSuperBubble(thunderBubble);
    }

}
