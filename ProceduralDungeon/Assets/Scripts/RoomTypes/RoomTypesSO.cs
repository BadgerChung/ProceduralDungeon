using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomType_", menuName = "RoomTypes")]
public class RoomTypesSO : ScriptableObject
{
    public int iterations = 10, walkLength = 10;
    public bool startRandomlyEachIteration = true; // pokud true, pøi každé iteraci zaène Random Walk z náhodné pozice, kterou už jednou vybral
}
