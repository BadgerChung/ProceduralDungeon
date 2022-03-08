using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasGrenade : MonoBehaviour
{

    public float cooldown;
    public float ttl;
    public float torque;
    public Wand wand;

    private float gasCooldown;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        gasCooldown -= Time.deltaTime;
        ttl -= Time.deltaTime;
        if(cooldown < 0)
        {
            rb.AddTorque(torque);
            if(gasCooldown < 0)
            {
                Shoot(wand);
                gasCooldown = 1f / wand.projectilesPerSecond;
            }
        }

        if (ttl < 0) Destroy(gameObject);
    }

    void Shoot(Wand wand)
    {
        Vector2 direction = transform.up + transform.right;
        direction.Normalize();
        Shoot(wand, direction);
        Shoot(wand, -direction);
    }

    void Shoot(Wand wand, Vector2 direction)
    {
        GameObject projectile = Instantiate(wand.projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        direction += new Vector2(Random.Range(-wand.spread, wand.spread), Random.Range(-wand.spread, wand.spread));
        direction.Normalize();
        rb.velocity = direction * wand.projectileSpeed;
    }
}
