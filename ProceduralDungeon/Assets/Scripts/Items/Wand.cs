using UnityEngine;

[CreateAssetMenu(fileName = "WandItem_", menuName = "Items/Wand")]
public class Wand : Item
{
    public GameObject projectile;
    public float projectileSpeed;
    public float projectilesPerSecond;
    public float spread;

}