using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMusicIntegratedTestMainMap : MonoBehaviour
{
    private float StartTime;
    private GameObject drone;
    private MainMapManager mainMapManager;
    private RiveAnimationManager riveAnimationManager;

    void Start()
    {
        StartTime = Time.time;
        drone = GameObject.FindWithTag("Player");
        mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        riveAnimationManager = GameObject.Find("RiveAnimationManager").GetComponent<RiveAnimationManager>();

        Debug.Log(riveAnimationManager.m_stateMachine[5].GetNumber("hp") != null);
    }

    void Update()
    {
        if (Time.time - StartTime > 5.0f)
        {
            Debug.Log(mainMapManager.audioSource.clip);
            riveAnimationManager.isMainMapMissionCleared[3] = true; // Set the mission as cleared to confirm music change.
            drone.GetComponent<DroneController>().MapClear();
        }
        if (Time.time - StartTime > 10.0f)
        {
            Debug.Log(mainMapManager.audioSource.clip);
            drone.GetComponent<DroneUIManager>().ShowMenuScreen();
        }
    }
}
