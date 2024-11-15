using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    private int enemyHp = 100;
    private bool isDead = false;
    Animator enemyAnimator;
    private ParticleSystem deathParticle;
    // private AudioSource enemyAudio;
    // public AudioClip enemyDeathAudio;
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        deathParticle = transform.Find("EnergyExplosion")?.GetComponent<ParticleSystem>();
        if (deathParticle == null)
        {
            Debug.LogWarning("No particle system named 'EnergyExplosion' found in the enemy object.");
        }
        else
        {
            deathParticle.Stop(); // Ensure it’s stopped at the start
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp == 0)
        {
            isDead = true;
            enemyAnimator.SetBool("Is_Death", true);
            Debug.Log("enemy is dead");
            Invoke("DestroyEnemy", 3f);
        }
    }

    void DestroyEnemy()
    {
        deathParticle.Play();
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
