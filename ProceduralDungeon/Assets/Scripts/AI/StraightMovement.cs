using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMovement : Movement
{

    public float stopDistance;

    public StraightMovement(Transform transform, Rigidbody2D rigidBody, float stopDistance) : base(transform, rigidBody) // konstruktor pro pohyb
    {
        this.stopDistance = stopDistance;
    }

    public override void Move(Vector2 moveTo, float speed) // pohyb
    {
        Vector2 position = transform.position;
        if(Vector2.Distance(position, moveTo) >= stopDistance)
        {
            Vector2 direction = (moveTo - position).normalized;
            rigidBody.velocity = direction * speed;
        }
    }

}
