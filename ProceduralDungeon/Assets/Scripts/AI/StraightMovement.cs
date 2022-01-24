using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMovement : Movement
{

    private float stopDistance;

    public StraightMovement(Transform transform, float stopDistance) : base(transform)
    {
        this.stopDistance = stopDistance;
    }

    public override void Move(Vector2 moveTo, float speed)
    {
        Vector2 position = transform.position;
        if(Vector2.Distance(position, moveTo) >= stopDistance)
        {
            Vector2 direction = (moveTo - position).normalized;
            transform.position += (Vector3)direction * Time.deltaTime * speed;
        }
    }

}
