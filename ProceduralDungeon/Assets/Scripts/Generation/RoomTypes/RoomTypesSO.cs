using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomType_", menuName = "RoomTypes")]
public class RoomTypesSO : ScriptableObject
{
    public int iterations = 10, walkLength = 10;
    public bool startRandomlyEachIteration = true; // pokud true, p�i ka�d� iteraci za�ne Random Walk z n�hodn� pozice, kterou u� jednou vybral
}
