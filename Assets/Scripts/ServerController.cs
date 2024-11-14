using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : InteractionController
{

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

}
