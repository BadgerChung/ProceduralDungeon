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
        if (interactables.Count > 0)
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

    private Interactable GetClosestInteractable()
    {
        float minDistance = float.MaxValue;
        Interactable closest = null;
        foreach (Interactable interactable in interactables)
        {
            if (Vector3.Distance(interactable.transform.position, player.transform.position) < minDistance)
            {
                closest = interactable;
            }
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
