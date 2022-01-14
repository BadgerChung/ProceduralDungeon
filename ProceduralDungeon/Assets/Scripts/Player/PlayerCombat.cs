using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    float projectileCooldown = 0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !InventoryVisualizer.instance.isInventoryOpen)
        {
            Item item = InventoryVisualizer.instance.selectedItem;
            if (item is Wand)
            {
                Wand wand = (Wand)item;
                if (projectileCooldown <= 0)
                {
                    Shoot(wand);
                    projectileCooldown = 1 / wand.projectilesPerSecond;
                }
            }
        }
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

    void Shoot(Wand wand)
    {
        Vector3 worldCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = worldCursorPosition - transform.position;
        direction.Normalize();
        GameObject projectile = Instantiate(wand.projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        direction += new Vector2(Random.Range(-wand.spread, wand.spread), Random.Range(-wand.spread, wand.spread));
        direction.Normalize();
        rb.velocity = direction * wand.projectileSpeed;
        Destroy(projectile, 3f);
    }
}
