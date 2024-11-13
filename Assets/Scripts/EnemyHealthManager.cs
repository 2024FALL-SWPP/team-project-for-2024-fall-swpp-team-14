using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    private int enemyHp = 100;
    private bool isDead = false;
    Animator enemyAnimator;
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHp == 0)
        {
            isDead = true;
            enemyAnimator.SetBool("Is_Death", true);
            Debug.Log("enemy is dead");
            Destroy(gameObject, 10f); //destroy enemy after 10 seconds
            //stop shooting
        }
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
