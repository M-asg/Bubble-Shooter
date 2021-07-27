using UnityEngine;


[CreateAssetMenu(menuName = "ShootingBubbleConfig")]
public class ShootingBubbleConfigs : ScriptableObject
{
    //field
    [SerializeField] float bubbleSpeed = 65f;

    [SerializeField] private Material aimMaterial;

    public Material AimMaterial
    {
        get => aimMaterial;
    }

    public float GetBubbleSpeed()
    {
        return bubbleSpeed;
    }
}