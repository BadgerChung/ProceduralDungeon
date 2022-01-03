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

    private Dictionary<string, Sprite> loadedSprites; // Naètené sprity itemù

    public Slot hoveringOver;
    private Item holding;

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

        // Vytvoøí všechny sloty otevøeného inventáøe
        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, openSlotsParent.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.itemImage.enabled = false;
            openInvSlots.Add(slot);
        }
        openSlotsParent.SetActive(false);

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
        InventoryChanged(playerInvSlots, inv);
    }

    private void OpenInventoryChanged(Inventory inv)
    {
        InventoryChanged(openInvSlots, inv);
    }

    private void InventoryChanged(List<Slot> invSlots, Inventory inv)
    {
        // Projede všechny sloty a zobrazí itemy
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
                ShiftClick(); // automatické prohození itemu z jednoho inventáøe do druhého
            }
            else
            {
                SwitchHoldingItemWithSlot(); // manuální prohození itemu z jednoho inventáøe do druhého
            }
        }
    }

    private void ShiftClick() // automaticky pøesune item na který kliknu do druhého inventáøe
    {
        if (openInventory == null) return;
        if (playerInvSlots.Contains(hoveringOver)) // pokud je to hráèùv inventáø
        {
            Item i = playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), null);
            if(!openInventory.TryAddItem(i))
            {
                playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), i); // pokud nelze pøidat item do druhého inventáøe, vrátí ho zpìt na své místo
            }
        }
        else // pokud to není hráèùv inventáø
        {
            Item i = openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), null);
            if (!playerInventory.TryAddItem(i))
            {
                openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), i); // pokud nelze pøidat item do druhého inventáøe, vrátí ho zpìt na své místo
            }
        }
    }

    private void SwitchHoldingItemWithSlot() // manuální pøesouvání itemù
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
