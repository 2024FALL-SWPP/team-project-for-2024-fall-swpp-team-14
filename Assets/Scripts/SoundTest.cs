using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    //Drone Damage Test
    private AudioSource playerAudio;
    public AudioClip droneDamageAudio;

    //NPC Laser Shoot Test
    public AudioClip enemyShootLaserAudio;
    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject fakeLaser = new GameObject("FakeLaser");
            fakeLaser.AddComponent<BoxCollider>();
            fakeLaser.tag = "Laser";
            Collider fakeCollider1 = fakeLaser.GetComponent<Collider>();
            OnTriggerEnter(fakeCollider1);// Call OnTriggerEnter manually.
            Destroy(fakeLaser);// Clean up.

            GameObject fakeLaserObstacle = new GameObject("FakeLaserObstacle");
            fakeLaserObstacle.AddComponent<BoxCollider>();
            Collider fakeCollider2 = fakeLaserObstacle.GetComponent<Collider>();
            fakeLaserObstacle.tag = "LaserObstacle";
            OnTriggerStay(fakeCollider2); // Call OnTriggerEnter manually.
            Destroy(fakeLaserObstacle); // Clean up.
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerAudio.PlayOneShot(enemyShootLaserAudio);
            Debug.Log("NPC Shoot Audio successfully played");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            playerAudio.PlayOneShot(droneDamageAudio);
            Debug.Log("Drone Damage Audio successfully played for Laser");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("LaserObstacle"))
        {
            playerAudio.PlayOneShot(droneDamageAudio);
            Debug.Log("Drone Damage Audio successfully played for Laser Obstacle");
        }
    }

}
