using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundTest : MonoBehaviour
{
    public GameObject drone;
    public GameObject enemyLaserPrefab;     // Reference to the Laser prefab
    private GameObject testLaser;
    public Vector3 laserStartPosition = new Vector3(68, 2, 17); // Starting position of the Laser
    public float laserSpeed = 10f;
    private EnemyLaserController enemyLaserController;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyLaserPrefab == null)
        {
            Debug.LogError("Drone or LaserPrefab is not assigned!");
            return;
        }
        testLaser = Instantiate(enemyLaserPrefab, laserStartPosition, Quaternion.identity);
        testLaser.transform.position = new Vector3(68, 2, 17);
        enemyLaserController = testLaser.GetComponent<EnemyLaserController>();
    }

    void Update()
    {
        if (enemyLaserController.getEnemyAudioShootPlayed())
        {
            Debug.Log("Enemy Shoot Audio Played Successully");
        }
    }
}
