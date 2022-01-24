using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Living
{

    protected Movement movement;
    public float speed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        movement = new StraightMovement(transform, 2);
    }

    // Update is called once per frame
    void Update()
    {
        movement.Move(GameManager.instance.player.transform.position, speed);
    }
}
