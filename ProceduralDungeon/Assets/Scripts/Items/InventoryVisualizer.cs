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

    [SerializeField]
    private SpriteRenderer handImage;

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
    private Dictionary<string, Sprite> loadedHandSprites;

    public Slot hoveringOver;
    private Item holding;

    private RectTransform textHolder;
    private Text header;
    private Text content;

    private void Awake()
    {
        instance = this;

        playerInvSlots = new List<Slot>();
        openInvSlots = new List<Slot>();
        loadedSprites = new Dictionary<string, Sprite>();
        loadedHandSprites = new Dictionary<string, Sprite>();

        textHolder = cursorFollower.transform.GetChild(0).GetComponent<RectTransform>();
        header = textHolder.transform.GetChild(0).GetComponent<Text>();
        content = textHolder.transform.GetChild(1).GetComponent<Text>();

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
        Sprite[] handSprites = Resources.LoadAll<Sprite>("HandItemSprites/");
        foreach (Sprite sprite in handSprites)
        {
            loadedHandSprites.Add(sprite.name, sprite);
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
                if (selectedItem != null) handImage.sprite = GetHandSpriteByName(selectedItem.systemName + "_hand");
                else handImage.sprite = null;
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
        // zobrazuje popisky u pøedmìtù
        cursorFollower.transform.position = Input.mousePosition;

        float pivotX = Input.mousePosition.x;
        if (pivotX > Screen.width / 2) pivotX = 1;
        else pivotX = 0;
        textHolder.pivot = new Vector2(pivotX, 1);

        if (hoveringOver != null && holding == null)
        {
            textHolder.gameObject.SetActive(true);
            if (playerInvSlots.Contains(hoveringOver))
            {
                Item item = playerInventory.slots[playerInvSlots.IndexOf(hoveringOver)];
                if(item != null)
                {
                    header.text = item.displayName;
                    content.text = item.lore;
                }
                else textHolder.gameObject.SetActive(false);
            }
            else if (openInvSlots.Contains(hoveringOver))
            {
                Item item = openInventory.slots[openInvSlots.IndexOf(hoveringOver)];
                if (item != null)
                {
                    header.text = item.displayName;
                    content.text = item.lore;
                }
                else textHolder.gameObject.SetActive(false);
            }
        }
        else
        {
            textHolder.gameObject.SetActive(false);
        }
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

        // pøepínání mezi sloty
        if(Input.mouseScrollDelta.y != 0)
        {
            selectedSlot -= (int)Input.mouseScrollDelta.y;
            if (selectedSlot >= playerInventory.slots.Length) selectedSlot = 0;
            if (selectedSlot < 0) selectedSlot = playerInventory.slots.Length - 1;
            PlayerInventoryChanged(playerInventory);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedSlot = 0;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedSlot = 1;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedSlot = 2;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            selectedSlot = 3;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            selectedSlot = 4;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            selectedSlot = 5;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha7))
        {
            selectedSlot = 6;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha8))
        {
            selectedSlot = 7;
            PlayerInventoryChanged(playerInventory);
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
            cursorFollower.enabled = false;
        }
        else
        {
            cursorFollower.enabled = true;
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

    public Sprite GetHandSpriteByName(string name)
    {
        if (loadedHandSprites.ContainsKey(name))
        {
            return loadedHandSprites[name];
        }
        else
        {
            return loadedHandSprites["default_hand"];
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
        if (openInvSlots.Contains(hoveringOver)) hoveringOver = null;
        openInventory = null;
        openSlotsParent.SetActive(false);
    }
    
}
