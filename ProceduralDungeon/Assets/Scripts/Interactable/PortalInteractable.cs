using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalInteractable : Interactable
{
    [SerializeField]
    private Text interactableText;

    public override void Activate()
    {
        interactableText.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        interactableText.gameObject.SetActive(false);
    }

    public override void Interact()
    {
        CurrentRun.currentRun = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
