using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombat : Combat
{

    private int damage;

    private float range;

    private float cooldown;
    private float currentCooldown;

    public MeleeCombat(Transform transform, int damage, float range, float cooldown) : base(transform)
    {
        this.damage = damage;
        this.range = range;
        this.cooldown = cooldown;
    }

    public override bool Update()
    {
        currentCooldown -= Time.deltaTime;
        if(target != null && currentCooldown < 0)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if(dist < range)
            {
                Attack();
                return false;
            }
            return true;
        }
        return false;
    }

    private void Attack()
    {
        target.Damage(damage);
        currentCooldown = cooldown;
        // ještì by to chtìlo animaci útoku
    }

}
