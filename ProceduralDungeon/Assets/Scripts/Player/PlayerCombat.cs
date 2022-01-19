using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Living
{

    float projectileCooldown = 0f;

    public override void Die()
    {
        Debug.Log("umøels");
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && !InventoryVisualizer.instance.isInventoryOpen)
        {
            Item item = InventoryVisualizer.instance.selectedItem;
            if (item is Wand)
            {
                Wand wand = (Wand)item;
                if(projectileCooldown <= 0)
                {
                    Shoot(wand);
                    projectileCooldown = 1 / wand.projectilesPerSecond;
                }
            }
        }
        projectileCooldown -= Time.deltaTime;
    }

    void Shoot(Wand wand) // støelba
    {
        Vector3 worldCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = worldCursorPosition - transform.position;
        direction.Normalize();
        GameObject projectile = Instantiate(wand.projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        direction += new Vector2(Random.Range(-wand.spread, wand.spread), Random.Range(-wand.spread, wand.spread));
        direction.Normalize();
        rb.velocity = direction * wand.projectileSpeed;
        ignoreList.Add(projectile);
        //Destroy(projectile, 3f);
    }
}
