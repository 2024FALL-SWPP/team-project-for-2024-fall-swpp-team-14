using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : InteractionController
{
    private MainMapManager mainMapManager;
    private DroneController droneController;
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
        droneController = GameObject.Find("Drone").GetComponent<DroneController>();
    }

    public override void StartInteraction()
    {
        if (droneController.droneGameState != DroneController.DroneGameState.InGame)
        {
            return;
        }

        Debug.Log("Server interaction succeed");
        isInteractionSuccessful = true;
        interactionManager.ExitInteraction(true);

        if (mainMapManager != null)
        {
            mainMapManager.isServerActivated = true;
        }
    }

}
