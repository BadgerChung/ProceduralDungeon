using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat
{

    protected Transform transform;
    protected Living target;

    public Combat(Transform transform) // transform hráèe
    {
        this.transform = transform;
    }

    public virtual void Target(Living target) // cíl útoku
    {
        this.target = target;
    }

    public abstract bool Update();

}
