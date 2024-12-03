using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserController : MonoBehaviour
{
    private float enemyLaserSpeed = 5.0f;
    private AudioSource enemyAudio;
    public AudioClip enemyShootAudio;
    private bool enemyShootAudioPlayed;
    // Start is called before the first frame update
    void Start()
    {
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.PlayOneShot(enemyShootAudio);
        enemyShootAudioPlayed = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * enemyLaserSpeed);
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Laser"))
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag("Enemy") && !collider.gameObject.CompareTag("Laser"))
        {
            Destroy(gameObject);
        }
    }

    public bool getEnemyAudioShootPlayed()
    {
        return enemyShootAudioPlayed;
    }
}
