using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Living
{

    public Movement movement;

    public Combat combat;

    public float speed;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rigidBody = GetComponent<Rigidbody2D>();

        movement = new PathMovement(transform, rigidBody, 1);
        combat = new MeleeCombat(transform, 10, 1.5f, 1f);
        combat.Target(GameManager.instance.player.GetComponent<Living>());
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = Vector2.zero;
        if(combat.Update())
        {
            movement.Move(GameManager.instance.player.transform.position, speed);
        }
    }
}