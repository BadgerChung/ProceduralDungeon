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
    private Color selectedSlotColor, defaultSlotColor;

    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private GameObject playerSlotsParent, openSlotsParent;

    [SerializeField]
    private Image cursorFollower;

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
    public Inventory openInventory;

    private List<Slot> openInvSlots;

    private Dictionary<string, Sprite> loadedSprites; // Na�ten� sprity item�

    public Slot hoveringOver;
    private Item holding;

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

        // Vytvo�� v�echny sloty otev�en�ho invent��e
        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, openSlotsParent.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.itemImage.enabled = false;
            openInvSlots.Add(slot);
        }
        openSlotsParent.SetActive(false);

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
        InventoryChanged(playerInvSlots, inv);
    }

    private void OpenInventoryChanged(Inventory inv)
    {
        InventoryChanged(openInvSlots, inv);
    }

    private void InventoryChanged(List<Slot> invSlots, Inventory inv)
    {
        // Projede v�echny sloty a zobraz� itemy
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if(i == selectedSlot && inv == playerInventory)
            {
                invSlots[i].GetComponent<Image>().color = selectedSlotColor;
            }
            else
            {
                invSlots[i].GetComponent<Image>().color = defaultSlotColor;
            }
            if(inv.slots[i] == null)
            {
                invSlots[i].itemImage.enabled = false;
            }
            else
            {
                invSlots[i].itemImage.enabled = true;
                invSlots[i].itemImage.sprite = GetSpriteByName(inv.slots[i].systemName);
            }
        }
    }

    public void Update()
    {
        cursorFollower.transform.position = Input.mousePosition;
        /*if(Input.GetKeyDown(KeyCode.E))
        {
            if(openInventory == null)
            {
                OpenInventory(new Inventory(10));
            }
            else
            {
                CloseInventory();
            }
        }*/
        if(Input.GetMouseButtonDown(0) && hoveringOver != null)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                ShiftClick(); // automatick� prohozen� itemu z jednoho invent��e do druh�ho
            }
            else
            {
                SwitchHoldingItemWithSlot(); // manu�ln� prohozen� itemu z jednoho invent��e do druh�ho
            }
        }
    }

    private void ShiftClick() // automaticky p�esune item na kter� kliknu do druh�ho invent��e
    {
        if (openInventory == null) return;
        if (playerInvSlots.Contains(hoveringOver)) // pokud je to hr���v invent��
        {
            Item i = playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), null);
            if(!openInventory.TryAddItem(i))
            {
                playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), i); // pokud nelze p�idat item do druh�ho invent��e, vr�t� ho zp�t na sv� m�sto
            }
        }
        else // pokud to nen� hr���v invent��
        {
            Item i = openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), null);
            if (!playerInventory.TryAddItem(i))
            {
                openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), i); // pokud nelze p�idat item do druh�ho invent��e, vr�t� ho zp�t na sv� m�sto
            }
        }
    }

    private void SwitchHoldingItemWithSlot() // manu�ln� p�esouv�n� item�
    {
        if (playerInvSlots.Contains(hoveringOver))
        {
            holding = playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), holding);
        }
        else
        {
            holding = openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), holding);
        }
        if (holding == null)
        {
            cursorFollower.gameObject.SetActive(false);
        }
        else
        {
            cursorFollower.gameObject.SetActive(true);
            cursorFollower.sprite = GetSpriteByName(holding.systemName);
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        if (loadedSprites.ContainsKey(name))
        {
            return loadedSprites[name];
        }
        else
        {
            return loadedSprites["default"];
        }
    }

    // WIP
    public void OpenInventory(Inventory inv)
    {
        openInventory = inv;
        openInventory.inventoryChanged += OpenInventoryChanged;
        openSlotsParent.SetActive(true);
        for(int i = 0; i < 20; i++)
        {
            if (i < inv.slots.Length) openInvSlots[i].gameObject.SetActive(true);
            else openInvSlots[i].gameObject.SetActive(false);
        }
        OpenInventoryChanged(openInventory);
    }

    public void CloseInventory()
    {
        if (openInventory == null) return;
        openInventory.inventoryChanged -= OpenInventoryChanged;
        openInventory = null;
        openSlotsParent.SetActive(false);
    }
    
}
