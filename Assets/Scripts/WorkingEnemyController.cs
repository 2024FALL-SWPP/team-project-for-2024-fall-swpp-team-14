using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingEnemyController : EnemyController
{
    public float rangeX, rangeZ;
    private Vector3 startPos, endPos, currentDest;

    public void setRangeX(float rangeX)
    {
        this.rangeX = rangeX;
    }
    public void setRangeZ(float rangeZ)
    {
        this.rangeZ = rangeZ;
    }
    public float getRangeX()
    {
        return this.rangeX;
    }
    public float getRangeZ()
    {
        return this.rangeZ;
    }

    public void Move()
    {
        float startDist = Vector3.Distance(transform.position, startPos);
        float endDist = Vector3.Distance(transform.position, endPos);

        if (Vector3.Distance(currentDest, startPos) < 0.01f && startDist < 0.5f)
        {
            currentDest = endPos;
            animator.SetTrigger("Is_Gathering");
        }
        else if (Vector3.Distance(currentDest, endPos) < 0.01f && endDist < 0.5f)
        {
            currentDest = startPos;
            animator.SetTrigger("Is_Gathering");
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Gathering"))
        {
            nmAgent.isStopped = true;
        }
        else
        {
            nmAgent.isStopped = false;
        }
        nmAgent.SetDestination(currentDest);
    }
    protected override void AlertZero()
    {
        initDistance = (initPosition - transform.position).magnitude;
        alertState = 0;
        animator.SetBool("Is_Aiming", false);
        animator.SetBool("Patrol", true);
        if (initDistance >= 0.5f && returnToWork)
        {
            nmAgent.SetDestination(initPosition);
        }
        else
        {
            returnToWork = false;
            Move();
        }
        delayCount = 2;
    }

    void Start()
    {
        startPos = new Vector3(initX, initY, initZ);
        endPos = new Vector3(rangeX, initY, rangeZ);
        currentDest = endPos;

        animator = GetComponent<Animator>();
        animator.SetBool("Patrol", true);

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
}
