using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat
{

    protected Transform transform;
    protected Living target;

    public Combat(Transform transform)
    {
        this.transform = transform;
    }

    public virtual void Target(Living target)
    {
        this.target = target;
    }

    public abstract bool Update();

}
