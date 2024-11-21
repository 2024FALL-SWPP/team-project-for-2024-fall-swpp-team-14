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
        mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
    }

    public override void StartInteraction()
    {
        Debug.Log("Server interaction succeed");
        isInteractionSuccessful = true;
        interactionManager.ExitInteraction(true);
        mainMapManager.isServerActivated = true;
    }

}
