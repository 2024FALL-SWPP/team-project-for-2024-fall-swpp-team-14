using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingEnemyController : EnemyController
{
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        nmAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        initPosition = new Vector3(initX, initY, initZ);
        lastDetectTime = -1000;
        lastAttackTime = -1000;
        alertState = 0;
        delayCount = 2;
        enemyHealthManager = GetComponent<EnemyHealthManager>();
        
        if (GameObject.Find("MainMapManager") != null)
        {
            mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        }
        else
        {
            mainMapManager = null;
        }
    }

    protected override void AlertZero()
    {
        initDistance = (initPosition - transform.position).magnitude;
        alertState = 0;
        animator.SetBool("Is_Aiming", false);
        nmAgent.isStopped = false;
        if (initDistance >= 0.5f)
        {
            animator.SetBool("Patrol", true);
            nmAgent.SetDestination(initPosition);
        }
        else
        {
            animator.SetBool("Patrol", false);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, initYRot, 0), Time.deltaTime * 2);
        }
        delayCount = 2;
    }
}