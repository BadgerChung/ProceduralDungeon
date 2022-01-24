using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{

    protected Transform transform;

    protected Movement(Transform transform)
    {
        this.transform = transform;
    }

    public abstract void Move(Vector2 moveTo, float speed);
}
