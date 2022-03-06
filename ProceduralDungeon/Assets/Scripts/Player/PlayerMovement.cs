using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private PlayerCombat playerCombat;

    [SerializeField]
    private Slider abilityBar;

    [SerializeField]
    private float moveSpeed = 5f;
    private Vector2 direction;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject shield;

    private bool ability;
    private float abilityCooldown;
    private float maxAbilityCooldown = 5f;
    private float abilityDuration;
    private string abilityType;

    private void Start()
    {
        GameManager.instance.player = gameObject;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        Move();
        Ability();
    }

    private void GetInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        float cursorX = Input.mousePosition.x;
        if (cursorX > Screen.width / 2) cursorX = 1;
        else cursorX = -1;

        direction = new Vector2(moveX, moveY).normalized;

        transform.localScale = new Vector3(cursorX, 1, 1);

        if (Input.GetKeyDown(KeyCode.Space))
            ability = true;
    }

    private void Move()
    {
        rb.velocity = direction * moveSpeed;
    }

    private void Ability()
    {
        abilityCooldown -= Time.deltaTime;
        abilityDuration -= Time.deltaTime;

        abilityBar.value = (maxAbilityCooldown - abilityCooldown) / maxAbilityCooldown;

        if (ability && abilityCooldown < 0)
        {
            if(InventoryVisualizer.instance.playerInventory.slots[7] is Accessory)
            {
                Accessory accessory = (Accessory) InventoryVisualizer.instance.playerInventory.slots[7];
                abilityCooldown = accessory.cooldown;
                maxAbilityCooldown = accessory.cooldown;
                if(accessory.accessoryType == "shield")
                {
                    abilityDuration = 2f;
                    abilityType = "shield";
                    playerCombat.shield = true;
                    shield.SetActive(true);
                }
            }
        }
        if(abilityDuration < 0)
        {
            if(abilityType == "shield")
            {
                playerCombat.shield = false;
                shield.SetActive(false);
                abilityType = "";
            }
        }
        ability = false;
    }
}