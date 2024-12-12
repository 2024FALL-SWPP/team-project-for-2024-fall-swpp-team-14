using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTest_MainMapInitialization : MonoBehaviour
{
    private GameObject drone;
    // Start is called before the first frame update
    void Start()
    {
        drone = GameObject.FindWithTag("Player");
        bool isSameCoordandRot = true;
        
        if (DataTransfer.skiptoMainmap3)
        {
            if (drone.transform.position != new Vector3(42, 11, -13))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, 130, 0))
            {
                isSameCoordandRot = false;
            }
        }
        else if (DataTransfer.skiptoMainmap2)
        {
            if (drone.transform.position != new Vector3(55, 5, -13))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, 0, 0))
            {
                isSameCoordandRot = false;
            }
        }
        else if (DataTransfer.skiptoMainmap1)
        {
            if (drone.transform.position != new Vector3(68, 2, 20))
            {
                isSameCoordandRot = false;
            }
            if (drone.transform.rotation != Quaternion.Euler(0, 180, 0))
            {
                isSameCoordandRot = false;
            }
        }

        Debug.Log("UnitTest_MainMapInitialization: " + isSameCoordandRot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
