using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{

    private List<Interactable> interactables;

    private Interactable lastClosest;

    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        interactables = new List<Interactable>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (interactables.Count > 0) // vybírá nejbližší interagovatlný objekt pokud je jich v blízkosti hráøe více
        {
            Interactable closest = GetClosestInteractable();
            if(lastClosest != closest)
            {
                if(lastClosest != null) lastClosest.Deactivate();
                closest.Activate();
                lastClosest = closest;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                lastClosest.Interact();
            }
        }
    }

    private Interactable GetClosestInteractable() // vracá nejbližší objekt
    {
        float minDistance = float.MaxValue;
        Interactable closest = null;
        foreach (Interactable interactable in interactables)
        {
            try
            {
                if (Vector3.Distance(interactable.transform.position, player.transform.position) < minDistance)
                {
                    closest = interactable;
                }
            }
            catch { }
        }
        return closest;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactables.Add(collision.GetComponent<Interactable>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactables.Remove(collision.GetComponent<Interactable>());
        if (interactables.Count == 0)
        {
            lastClosest.Deactivate();
            lastClosest = null;
        }
    }
}
