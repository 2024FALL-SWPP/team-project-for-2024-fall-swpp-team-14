using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapClearIntegratedTest : MonoBehaviour
{
    public RiveScreenTutorial riveScreenTutorial;
    public DroneController droneController;
    public DroneUIManager droneUIManager;
    public float testTime = 0f;
    public bool missionBoolsChanged = false;
    public bool droneGameStateTested = false;

    // Start is called before the first frame update
    void Start()
    {
        riveScreenTutorial = GameObject.Find("Drone").GetComponentInChildren<RiveScreenTutorial>();
        droneController = GameObject.Find("Drone").GetComponent<DroneController>();
        droneUIManager = GameObject.Find("Drone").GetComponent<DroneUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        testTime += Time.deltaTime;
        if (testTime >= 3 && !missionBoolsChanged)
        {
            checkDroneGameState(DroneController.DroneGameState.InGame);
            checkMapClearScreenShowed(false);
            for (int i = 0; i < riveScreenTutorial.missionBools.Length; i++)
            {
                riveScreenTutorial.missionBools[i].Value = true;
            }
            missionBoolsChanged = true;
        }
        if (testTime >= 10 && !droneGameStateTested)
        {
            checkDroneGameState(DroneController.DroneGameState.MapClear);
            checkMapClearScreenShowed(true);
            droneGameStateTested = true;
        }
    }

    void checkDroneGameState(DroneController.DroneGameState expectedDroneGameState)
    {
        Debug.Assert(droneController.droneGameState == expectedDroneGameState);
        Debug.Log("[checkDroneGameState] expected state: " + expectedDroneGameState + ", current state: " + droneController.droneGameState);
    }

    void checkMapClearScreenShowed(bool expected)
    {
        Debug.Assert(droneUIManager.hasMapClearScreenShown == expected);
        Debug.Log("[checkMapClearScreenShowed] expected : " + expected + ", current : " + droneUIManager.hasMapClearScreenShown);
    }
}