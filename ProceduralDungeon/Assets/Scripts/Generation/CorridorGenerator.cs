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

    public static void GenerateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions) // do prom�nn� floorPositions ukl�d� pozice vygenerovan� koridor�, do prom�nn� potentialRoomPositions ukl�d� pozice konc� koridor�, neboli potenci�ln� pozice pro m�stnosti
    {
        Vector2Int currentPosition = startPosition;
        Vector2Int direction = Direction2D.GetRandomDirection();
        potentialRoomPositions.Add(currentPosition);

        for (int x = 0; x < branches.Length; x++)
        {
            for (int i = 0; i < branches[x]; i++)
            {
                List<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength, direction);
                currentPosition = path[path.Count - 1]; // od path.Count se ode��t� 1, aby dal�� prom�nn� currentPosition byla stejn� jako posledn� pozice prom�nn� path (prom�nn� path je typu List, po�ad� v List za��n� od 0, vlastnost Count za��n� od 1)
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(path);
                DungeonGenerator.instance.corridorsPositions.UnionWith(path);

                int number = Random.Range(0, 3); // 
                if (number == 1) direction = Direction2D.TurnLeft(direction);
                else if (number == 2) direction = Direction2D.TurnRight(direction);
                // tento blok k�du �e��, aby se nevygenerovaly duplik�ty pozic koridor� (nap�. jeden by se vygeneroval sm�rem doprava, a druh� by se vygeneroval na stejn� pozice zp�tky sm�rem doleva)
                // n�hodn� se vyb�r�, zdali sm�r vektoru dal��ho koridoru se oto�� o 90� doprava, doleva nebo bude pokra�ovat ve stejn�m sm�ru (rovn�)
                // d�ky tomu se pozice dal��ho koridoru nikdy nevygeneruj� zp�t (opa�n�m sm�rem) na pozice ji� vygenerovan�
            }

            currentPosition = potentialRoomPositions.ElementAt(Random.Range(0, potentialRoomPositions.Count()));
        }
    }

    public static HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> potentialRoomPositions) // z prom�nn� potentialRoomPositions vybere pozice pro m�stnosti a vrac� je
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomsCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); // vybere po�et m�stnost� podle roomPercent

        List<Vector2Int> rooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsCount).ToList(); // nejd��ve se�ad� prvky z potentialRoomPositions pomoc� guid 
                                                                                                                // (guid ka�d�mu prvku p�i�ad� n�hodn� jedine�n� id, d�ky �emu� se prvky se�ad� n�hodn�)
                                                                                                                // pot� z nich vybere n�kolik prvk� podle roomsCount
        HashSet<Vector2Int> startRoomFloor = new HashSet<Vector2Int>(); // tento blok vytv��� pozice pro startovn� m�stnost dle parametr� v DungeonGenerator
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
            HashSet<Vector2Int> roomFloor = DungeonGenerator.instance.RunRandomWalk(roomPosition); // na ka�d� pozici z List rooms vygeneruje pozice pro m�stnosti pomoc� funkce RunRandomWalk()
            if(roomPosition == Vector2Int.zero) // kdyby se na startovn� pozici vygenerovala jin� m�stnost, tak se p�id� ke startovn� m�stnosti
            {
                DungeonGenerator.instance.roomsPositions[0].UnionWith(roomFloor);
            }
            else
            {
                DungeonGenerator.instance.roomsPositions.Add(roomFloor); // v�echny ostatn� m�stnosti se p�id�v�j� do listu kv�li mo�nosti rozli�ov�n� jednotliv�ch m�stnost�
            }
            
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    public static HashSet<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorPositions) // vrac� pozice konc� slep�ch koridor�
    {
        HashSet<Vector2Int> deadEnds = new HashSet<Vector2Int>();
        foreach (Vector2Int position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (Vector2Int direction in Direction2D.directionsList)
            {
                if (floorPositions.Contains(position + direction)) // prohled� pozice z floorPositions do v�ech sm�r� a pokud n�jak� z nich u� ve floorPositions je, zv�t�� se neighbourCount o 1
                    neighbourCount++;
            }

            if (neighbourCount == 1) // pokud je soused jen jeden, znamen� to, �e je to slep� koridor, proto�e jedin� soused je prvn� pozice sm�rem k za��tku koridoru
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    public static void FixDeadEnds(HashSet<Vector2Int> deadEnds, HashSet<Vector2Int> roomPositions) // �e�� slep� koridory t�m, �e pozice z prom�nn� deadEnds p�id� do pozic pro m�stnosti
    {
        foreach (Vector2Int position in deadEnds)
        {
            if (roomPositions.Contains(position) == false) // pokud pozice konce slep�ho koridoru nen� pozic� pro m�stnost, stane se pozic� pro m�stnost
            {
                HashSet<Vector2Int> roomPosition = DungeonGenerator.instance.RunRandomWalk(position);
                roomPositions.UnionWith(roomPosition);
            }
        }
    }
}
