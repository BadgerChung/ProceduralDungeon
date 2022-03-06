using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{

    SpriteRenderer sr;
    float ttl = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ttl -= Time.deltaTime;

        sr.color = new Color(1,1,1, ttl/1f);

        if (ttl < 0) Destroy(gameObject);
    }
}
