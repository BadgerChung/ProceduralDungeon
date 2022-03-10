using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int damage;
    public bool penetration;
    public float ttl;

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0) Destroy(gameObject); // pokud existuje projektil pøíliš dlouho, znièí se
    }

}
