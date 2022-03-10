using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : Living
{

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private GameObject deathScreen;

    float maxhp;
    float projectileCooldown = 0f;
    bool dead;

    public bool shield;

    public override void Die() // smrt
    {
        if (dead) return;

        Instantiate(deathEffect, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        dead = true;
        CurrentRun.currentRun = false;
        deathScreen.SetActive(true); // aktivace obrazovky po smrti
    }

    public override void Damage(int damage) // zran�n�
    {
        if (shield) return; // pokud je aktivn� �t�t, hr�� nen� zran�n
        base.Damage(damage);
        healthBar.value = hp / maxhp;
    }

    protected override void Start()
    {
        base.Start();
        maxhp = hp;
        if(CurrentRun.currentRun)
        {
            hp = CurrentRun.playerHp;
            healthBar.value = hp / maxhp;
        }
    }

    protected override void Update()
    {
        base.Update();
        // nastavov�n� p�edm�t� podle toho, jak� je vybran�
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
            else if (item is Consumable)
            {
                Consumable consumable = (Consumable)item;
                
                hp += consumable.hpReplenish;
                if (hp > maxhp) hp = (int)maxhp;
                healthBar.value = hp / maxhp;

                InventoryVisualizer.instance.playerInventory.SwitchSlot(InventoryVisualizer.instance.selectedSlot, null);
            }
            else if (item is Throwable)
            {
                Throwable throwable = (Throwable)item;

                GameObject g = Instantiate(throwable.prefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
                Vector3 worldCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = worldCursorPosition - transform.position;
                direction.Normalize();
                rb.velocity = direction * 10f;

                InventoryVisualizer.instance.playerInventory.SwitchSlot(InventoryVisualizer.instance.selectedSlot, null);
            }
        }
        projectileCooldown -= Time.deltaTime;
    }

    void Shoot(Wand wand) // st�elba
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
    }

    private void OnDestroy()
    {
        CurrentRun.playerHp = hp;
    }
}
