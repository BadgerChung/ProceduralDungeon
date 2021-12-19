using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CorridorGenerator
{
    public static Vector2Int startPosition;
    public static int corridorLength;
    public static int[] branches;

    [Range(0.1f, 1)]
    public static float roomPercent;

    public static void GenerateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions) // do promìnné floorPositions ukládá pozice vygenerované koridorù, do promìnné potentialRoomPositions ukládá pozice koncù koridorù, neboli potenciální pozice pro místnosti
    {
        Vector2Int currentPosition = startPosition;
        Vector2Int direction = Direction2D.GetRandomDirection();
        potentialRoomPositions.Add(currentPosition);

        for (int x = 0; x < branches.Length; x++)
        {
            for (int i = 0; i < branches[x]; i++)
            {
                List<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength, direction);
                currentPosition = path[path.Count - 1]; // od path.Count se odeèítá 1, aby další promìnná currentPosition byla stejná jako poslední pozice promìnné path (promìnná path je typu List, poøadí v List zaèíná od 0, vlastnost Count zaèíná od 1)
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(path);
                DungeonGenerator.instance.corridorsPositions.UnionWith(path);

                int number = Random.Range(0, 3); // 
                if (number == 1) direction = Direction2D.TurnLeft(direction);
                else if (number == 2) direction = Direction2D.TurnRight(direction);
                // tento blok kódu øeší, aby se nevygenerovaly duplikáty pozic koridorù (napø. jeden by se vygeneroval smìrem doprava, a druhý by se vygeneroval na stejné pozice zpátky smìrem doleva)
                // náhodnì se vybírá, zdali smìr vektoru dalšího koridoru se otoèí o 90° doprava, doleva nebo bude pokraèovat ve stejném smìru (rovnì)
                // díky tomu se pozice dalšího koridoru nikdy nevygenerují zpìt (opaèným smìrem) na pozice již vygenerované
            }

            currentPosition = potentialRoomPositions.ElementAt(Random.Range(0, potentialRoomPositions.Count()));
        }
    }

    public static HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> potentialRoomPositions) // z promìnné potentialRoomPositions vybere pozice pro místnosti a vrací je
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomsCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); // vybere poèet místností podle roomPercent

        List<Vector2Int> rooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsCount).ToList(); // nejdøíve seøadí prvky z potentialRoomPositions pomocí guid 
                                                                                                                // (guid každému prvku pøiøadí náhodné jedineèné id, díky èemuž se prvky seøadí náhodnì)
                                                                                                                // poté z nich vybere nìkolik prvkù podle roomsCount
        HashSet<Vector2Int> startRoomFloor = new HashSet<Vector2Int>(); // tento blok vytváøí pozice pro startovní místnost dle parametrù v DungeonGenerator
        for (int y = -DungeonGenerator.instance.startRoomRectHeight; y < DungeonGenerator.instance.startRoomRectHeight + 1; y++)
        {
            for (int x = -DungeonGenerator.instance.startRoomRectWidth; x < DungeonGenerator.instance.startRoomRectWidth + 1; x++)
            {
                startRoomFloor.Add(new Vector2Int(x, y));
            }
        }
        roomPositions.UnionWith(startRoomFloor);
        DungeonGenerator.instance.roomsPositions.Add(startRoomFloor);

        foreach (Vector2Int roomPosition in rooms)
        {
            HashSet<Vector2Int> roomFloor = DungeonGenerator.instance.RunRandomWalk(roomPosition); // na každé pozici z List rooms vygeneruje pozice pro místnosti pomocí funkce RunRandomWalk()
            if(roomPosition == Vector2Int.zero) // kdyby se na startovní pozici vygenerovala jiná místnost, tak se pøidá ke startovní místnosti
            {
                DungeonGenerator.instance.roomsPositions[0].UnionWith(roomFloor);
            }
            else
            {
                DungeonGenerator.instance.roomsPositions.Add(roomFloor); // všechny ostatní místnosti se pøidávájí do listu kvùli možnosti rozlišování jednotlivých místností
            }
            
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    public static HashSet<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorPositions) // vrací pozice koncù slepých koridorù
    {
        HashSet<Vector2Int> deadEnds = new HashSet<Vector2Int>();
        foreach (Vector2Int position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (Vector2Int direction in Direction2D.directionsList)
            {
                if (floorPositions.Contains(position + direction)) // prohledá pozice z floorPositions do všech smìrù a pokud nìjaká z nich už ve floorPositions je, zvìtší se neighbourCount o 1
                    neighbourCount++;
            }

            if (neighbourCount == 1) // pokud je soused jen jeden, znamená to, že je to slepý koridor, protože jediný soused je první pozice smìrem k zaèátku koridoru
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    public static void FixDeadEnds(HashSet<Vector2Int> deadEnds, HashSet<Vector2Int> roomPositions) // øeší slepé koridory tím, že pozice z promìnné deadEnds pøidá do pozic pro místnosti
    {
        foreach (Vector2Int position in deadEnds)
        {
            if (roomPositions.Contains(position) == false) // pokud pozice konce slepého koridoru není pozicí pro místnost, stane se pozicí pro místnost
            {
                HashSet<Vector2Int> roomPosition = DungeonGenerator.instance.RunRandomWalk(position);
                roomPositions.UnionWith(roomPosition);
            }
        }
    }
}
