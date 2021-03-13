using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="ShootingBubbleConfig")]
public class ShootingBubbleConfigs : ScriptableObject
{
    //field
    [SerializeField] float bubbleSpeed = 65f;


    //property
    public float GetBubbleSpeed()
    {
        return bubbleSpeed;
    }
}
