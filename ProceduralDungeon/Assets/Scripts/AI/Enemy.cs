using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Living
{

    public Movement movement;

    public Combat combat;

    public float speed;

    private float speedMultiplier = 1;

    private float damageCooldown = 0;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rigidBody = GetComponent<Rigidbody2D>();

        movement = new PathMovement(transform, rigidBody, 0.7f);
        combat = new MeleeCombat(transform, 10, 1.5f, 0.5f);
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