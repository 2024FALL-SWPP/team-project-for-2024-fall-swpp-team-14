using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSoundTest : MonoBehaviour
{
    public GameObject drone;           // Reference to the Drone GameObject
    public GameObject enemyLaserPrefab;     // Reference to the Laser prefab
    private GameObject testLaser;
    public Vector3 laserStartPosition = new Vector3(68, 2, 17); // Starting position of the Laser
    public float laserSpeed = 10f;
    private DroneController droneController;

    // Start is called before the first frame update
    void Start()
    {
        if (drone == null || enemyLaserPrefab == null)
        {
            Debug.LogError("Drone or LaserPrefab is not assigned!");
            return;
        }
        drone.transform.position = new Vector3(68, 2, 20);
        testLaser = Instantiate(enemyLaserPrefab, laserStartPosition, Quaternion.identity);
        testLaser.transform.position = new Vector3(68, 2, 17);
        droneController = drone.GetComponent<DroneController>();
    }

    void Update()
    {
        if (droneController.getDamageAudioWasPlayed())
        {
            Debug.Log("Drone Damage Audio Played Successully");
        }
    }

}