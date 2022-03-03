using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living : MonoBehaviour
{

    public int hp;

    protected List<GameObject> ignoreList;

    protected SpriteRenderer sprite;

    protected float redness;

    protected virtual void Start()
    {
        ignoreList = new List<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        redness -= Time.deltaTime * 2f;
        redness = Mathf.Clamp01(redness);
        sprite.color = new Color(1, 1 - redness, 1 - redness, 1);
    }

    public virtual void Damage(int damage)
    {
        hp -= damage;
        if(hp < 1)
        {
            Die();
        }
        redness = 1;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.GetComponent<Projectile>();
        if(projectile != null)
        {
            if (ignoreList.Contains(collision.gameObject)) return;
            Damage(projectile.damage);
            if (!projectile.penetration) Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ignoreList.Remove(collision.gameObject);
    }
}
