using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTest_TutorialMapInitialization : MonoBehaviour
{
    private GameObject drone;
    // Start is called before the first frame update
    void Start()
    {
        drone = GameObject.FindWithTag("Player");
        bool isSameCoordandRot = true;

        if (DataTransfer.skiptoTutorial3)
        {
            if (drone.transform.position != new Vector3(3, 0, -7))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, -90, 0))
            {
                isSameCoordandRot = false;
            }
        }
        else if (DataTransfer.skiptoTutorial2)
        {
            if (drone.transform.position != new Vector3(6, -0.5f, -3))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, 180, 0))
            {
                isSameCoordandRot = false;
            }
        }
        else if (DataTransfer.skiptoTutorial1)
        {
            if (drone.transform.position != new Vector3(11, 0, 6))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, -90, 0))
            {
                isSameCoordandRot = false;
            }
        }

        Debug.Log("UnitTest_TutorialMapInitialization: " + isSameCoordandRot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
