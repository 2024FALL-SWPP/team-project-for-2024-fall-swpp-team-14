using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPIntegrateTest : MonoBehaviour
{
    public DroneController drone;
    public RiveAnimationManager riveAnimationManager;
    // Start is called before the first frame update
    void Start()
    {
        drone = GameObject.Find("Drone").GetComponent<DroneController>();
        riveAnimationManager = GameObject.Find("RiveAnimationManager").GetComponent<RiveAnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((drone.droneHp / 10) != riveAnimationManager.hp.Value)
        {
            Debug.Log("Drone HP not corresponding to Rive Animation");
        }
        if (drone.droneHp > 1)
        {
            drone.droneHp -= 1;
        }
    }
}
