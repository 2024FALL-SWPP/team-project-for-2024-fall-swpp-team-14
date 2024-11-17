using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionController : MonoBehaviour
{
    protected bool isInteractionSuccessful = false;
    protected InteractionManager interactionManager;
    // Start is called before the first frame update
    void Awake()
    {
        interactionManager = GameObject.Find("InteractionManager").GetComponent<InteractionManager>();
    }
    public abstract void StartInteraction();

    public bool CanInteract()
    {
        return !isInteractionSuccessful;
    }
}
