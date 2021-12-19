using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisualizer : MonoBehaviour
{
    public static InventoryVisualizer instance;

    [SerializeField]
    private int playerSlots; // Po�et hr��ov�ch slot�

    [SerializeField]
    private Item[] startItems; // Startovn� itemy

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

    private Dictionary<string, Sprite> loadedSprites; // Na�ten� sprity item�

    private void Awake()
    {
        instance = this;

        playerInvSlots = new List<Slot>();
        openInvSlots = new List<Slot>();
        loadedSprites = new Dictionary<string, Sprite>();

        // Vytvo�� v�echny sloty hr��ova invent��e
        for (int i = 0; i < playerSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, playerSlotsParent.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.itemImage.enabled = false;
            playerInvSlots.Add(slot);
        }

        // Naloadov�n� sprit�
        Sprite[] sprites = Resources.LoadAll<Sprite>("ItemSprites/");
        foreach(Sprite sprite in sprites)
        {
            loadedSprites.Add(sprite.name, sprite);
        }

        // Setup hr��ova invent��e (objektu)
        playerInventory = new Inventory(playerSlots);
        playerInventory.inventoryChanged += PlayerInventoryChanged;
        selectedSlot = 0;

        // P�id�n� startovn�ch item� do hr��ova invent��e
        foreach (Item item in startItems) playerInventory.TryAddItem(item);
    }


    private void PlayerInventoryChanged(Inventory inv)
    {
        // Projede v�echny sloty a zobraz� itemy
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
