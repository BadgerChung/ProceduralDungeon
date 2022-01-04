using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Item")]
public class Item : ScriptableObject
{

    public string displayName;
    public string systemName;

    [TextArea(1,10)]
    public string lore;

}
