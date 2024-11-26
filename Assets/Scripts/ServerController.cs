using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : InteractionController
{
    private MainMapManager mainMapManager;
    // Update is called once per frame
    void Update()
    {

    }

    void Start()
    {
        if (GameObject.Find("MainMapManager") != null)
        {
            mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        }
        else
        {
            mainMapManager = null;
        }    
    }

    public override void StartInteraction()
    {
        Debug.Log("Server interaction succeed");
        isInteractionSuccessful = true;
        interactionManager.ExitInteraction(true);

        if (mainMapManager != null)
        {
            mainMapManager.isServerActivated = true;
        }
    }

}
