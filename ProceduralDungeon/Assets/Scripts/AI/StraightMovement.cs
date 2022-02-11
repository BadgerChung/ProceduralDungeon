using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMovement : Movement
{

    private float stopDistance;

    public StraightMovement(Transform transform, Rigidbody2D rigidBody, float stopDistance) : base(transform, rigidBody)
    {
        this.stopDistance = stopDistance;
    }

    public override void Move(Vector2 moveTo, float speed)
    {
        Vector2 position = transform.position;
        if(Vector2.Distance(position, moveTo) >= stopDistance)
        {
            Vector2 direction = (moveTo - position).normalized;
            rigidBody.velocity = direction * speed;
            //transform.position += (Vector3)direction * Time.deltaTime * speed;
        }
    }

}
