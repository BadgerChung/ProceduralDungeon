using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{

    private ParticleSystem ps;
    public float ttl = 2f;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play(); // pøehraje efekt smrti
    }

    private void Update()
    {
        ttl -= Time.deltaTime;

        if (ttl < 0) Destroy(gameObject);
    }
}
