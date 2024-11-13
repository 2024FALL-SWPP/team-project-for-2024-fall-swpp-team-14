using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : InteractionController
{
    // private bool isInteractionSuccessful = false;
    // private InteractionManager interactionManager;

    // Start is called before the first frame update
    // void Start()
    // {
    //     interactionManager = GameObject.Find("InteractionManager").GetComponent<InteractionManager>();
    // }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartInteraction()
    {
        Debug.Log("Server interaction succeed");
        isInteractionSuccessful = true;
        interactionManager.ExitInteraction(true);
    }

    //     public bool CanInteract()
    //     {
    //         return !isInteractionSuccessful;
    //     }
}
