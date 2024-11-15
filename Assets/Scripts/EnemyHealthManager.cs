using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    private int enemyHp = 100;
    private bool isDead = false;
    Animator enemyAnimator;
    private ParticleSystem deathParticle;
    private AudioSource enemyAudio;
    public AudioClip enemyDeathAudio;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        deathParticle = transform.Find("EnergyExplosion")?.GetComponent<ParticleSystem>();
        enemyAudio = Camera.main.GetComponent<AudioSource>();
        if (deathParticle == null || enemyAudio == null || enemyDeathAudio == null)
        {
            Debug.LogWarning("Particle or audio is null");
        }
        enemyAudio.ignoreListenerPause = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp == 0)
        {
            isDead = true;
            enemyAnimator.SetBool("Is_Death", true);
            Invoke("DestroyEnemy", 3f);
        }
    }

    void DestroyEnemy()
    {
        deathParticle.Play();
        if (!enemyAudio.isPlaying)
        {
            enemyAudio.PlayOneShot(enemyDeathAudio);
        }
        Destroy(gameObject, 0.8f); //destroy enemy after 10 seconds
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DroneLaser")) //if hit by drone's laser, decrease HP
        {
            enemyHp -= 20;
            Debug.Log("enemy hp decrease");
        }
    }

    public bool checkDeath()
    {
        return this.isDead;
    }
}
