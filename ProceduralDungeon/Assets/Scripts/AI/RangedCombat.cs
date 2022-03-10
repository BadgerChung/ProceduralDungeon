using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedCombat : Combat
{

    public int damage;

    public float range;

    public float cooldown;

    public GameObject projectile;
    public float projectileSpeed;

    private float currentCooldown;

    public RangedCombat(Transform transform, int damage, float range, float cooldown, GameObject projectile, float projectileSpeed) : base(transform) // konstruktor pro boj na dálku
    {
        this.damage = damage;
        this.range = range;
        this.cooldown = cooldown;
        this.projectile = projectile;
        this.projectileSpeed = projectileSpeed;
    }

    public override bool Update()
    {
        currentCooldown -= Time.deltaTime;
        if (target != null && currentCooldown < 0)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < range) // pokud je cíl v dosahu, spustí se útok
            {
                Attack();
                currentCooldown = cooldown;
                return false;
            }
            return true;
        }
        return false;
    }

    private void Attack() // útok
    {
        Vector3 targetPosition = target.transform.position;
        Vector2 direction = targetPosition - transform.position;
        direction.Normalize();
        GameObject proj = Object.Instantiate(projectile, transform.position + (Vector3)direction, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }

}
