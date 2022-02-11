using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{

    protected Transform transform;
    protected Rigidbody2D rigidBody;

    protected Movement(Transform transform, Rigidbody2D rigidBody)
    {
        this.transform = transform;
        this.rigidBody = rigidBody;
    }

    public abstract void Move(Vector2 moveTo, float speed);
}
