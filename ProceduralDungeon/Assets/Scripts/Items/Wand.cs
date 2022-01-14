using UnityEngine;

[CreateAssetMenu(fileName = "WandItem_", menuName = "Wand")]
public class Wand : Item
{

    public int damage;

    public GameObject projectile;
    public float projectileSpeed;
    public float projectilesPerSecond;
    public float spread;

}