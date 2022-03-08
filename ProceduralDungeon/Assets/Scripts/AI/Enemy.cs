using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Living
{

    public Movement movement;

    public Combat combat;

    public bool ranged;

    public float speed;
    public float stopDistance;
    public int damage;
    public float range;
    public float cooldown;
    public GameObject projectile;
    public float projectileSpeed;

    private float speedMultiplier = 1;

    private float damageCooldown = 0;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rigidBody = GetComponent<Rigidbody2D>();

        movement = new PathMovement(transform, rigidBody, stopDistance);

        if(ranged)
        {
            combat = new RangedCombat(transform, damage, range, cooldown, projectile, projectileSpeed);
        }
        else
        {
            combat = new MeleeCombat(transform, damage, range, cooldown);
        }
        
        combat.Target(GameManager.instance.player.GetComponent<Living>());
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        speedMultiplier = 0.5f;
        damageCooldown = 1f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        damageCooldown -= Time.deltaTime;
        if(damageCooldown < 0)
        {
            speedMultiplier = 1;
        }
        rigidBody.velocity = Vector2.zero;
        if(combat.Update())
        {
            movement.Move(GameManager.instance.player.transform.position, speed * speedMultiplier);
        }
    }
}