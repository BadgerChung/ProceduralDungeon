using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisualizer : MonoBehaviour
{
    public static InventoryVisualizer instance;

    [SerializeField]
    private int playerSlots; // Poèet hráèových slotù

    [SerializeField]
    private Item[] startItems; // Startovní itemy

    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private GameObject playerSlotsParent;

    private Inventory playerInventory;
    private int selectedSlot;

    public Item selectedItem
    {
        get
        {
            return playerInventory.slots[selectedSlot];
        }
    }

    private List<Slot> playerInvSlots;

    // WIP
    private Inventory openInventory;

    private List<Slot> openInvSlots;

    private Dictionary<string, Sprite> loadedSprites; // Naètené sprity itemù

    private void Awake()
    {
        instance = this;

        playerInvSlots = new List<Slot>();
        openInvSlots = new List<Slot>();
        loadedSprites = new Dictionary<string, Sprite>();

        // Vytvoøí všechny sloty hráèova inventáøe
        for (int i = 0; i < playerSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, playerSlotsParent.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.itemImage.enabled = false;
            playerInvSlots.Add(slot);
        }

        // Naloadování spritù
        Sprite[] sprites = Resources.LoadAll<Sprite>("ItemSprites/");
        foreach(Sprite sprite in sprites)
        {
            loadedSprites.Add(sprite.name, sprite);
        }

        // Setup hráèova inventáøe (objektu)
        playerInventory = new Inventory(playerSlots);
        playerInventory.inventoryChanged += PlayerInventoryChanged;
        selectedSlot = 0;

        // Pøidání startovních itemù do hráèova inventáøe
        foreach (Item item in startItems) playerInventory.TryAddItem(item);
    }


    private void PlayerInventoryChanged(Inventory inv)
    {
        // Projede všechny sloty a zobrazí itemy
        for (int i = 0; i < playerSlots; i++)
        {
            if(i == selectedSlot)
            {
                playerInvSlots[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                playerInvSlots[i].GetComponent<Image>().color = Color.black;
            }
            if(inv.slots[i] == null)
            {
                playerInvSlots[i].itemImage.enabled = false;
            }
            else
            {
                playerInvSlots[i].itemImage.enabled = true;
                if (loadedSprites.ContainsKey(inv.slots[i].systemName))
                {
                    playerInvSlots[i].itemImage.sprite = loadedSprites[inv.slots[i].systemName];
                }
                else
                {
                    playerInvSlots[i].itemImage.sprite = loadedSprites["default"];
                }
            }
        }
    }

    // WIP
    public void OpenInventory(Inventory inv)
    {
        openInventory = inv;
    }
    
}
