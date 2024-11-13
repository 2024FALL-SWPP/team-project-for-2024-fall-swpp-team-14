using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyController : MonoBehaviour
{
    public GameObject player;
    public Transform playerPosition;
    public float initX, initY, initZ, initYRot;
    public float rangeX, rangeZ, speed;
    private Animator animator;
    private Vector3 direction;
    private LayerMask obstacleLayerMask = 1 << 6;
    private UnityEngine.AI.NavMeshAgent nmAgent;
    private float lastDetectTime;
    private float lastAttackTime;
    private Vector3 initPosition;
    public float initDistance;
    private GameObject laserPrefab;
    private int alertState = 0;
    private Vector3 startPos, endPos, currentDest;
    private int delayCount = 2;

    private bool returnToWork = false;

    private EnemyHealthManager enemyHealthManager;

    public void setInitX(float initx)
    {
        this.initX = initx;
    }
    public void setInitY(float inity)
    {
        this.initY = inity;
    }
    public void setInitZ(float initz)
    {
        this.initZ = initz;
    }
    public void setInitYRot(float inityrot)
    {
        this.initYRot = inityrot;
    }
    public void setLaserPrefab(GameObject laserPrefab)
    {
        this.laserPrefab = laserPrefab;
    }
    public float getInitX()
    {
        return this.initX;
    }
    public float getInitY()
    {
        return this.initY;
    }
    public float getInitZ()
    {
        return this.initZ;
    }
    public float getInitYRot()
    {
        return this.initYRot;
    }
    public void setRangeX(float rangeX)
    {
        this.rangeX = rangeX;
    }
    public void setRangeZ(float rangeZ)
    {
        this.rangeZ = rangeZ;
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public float getRangeX()
    {
        return this.rangeX;
    }
    public float getRangeZ()
    {
        return this.rangeZ;
    }
    public float getSpeed()
    {
        return this.speed;
    }

    bool IsVisible(Vector3 toPosition, int distanceLimit)
    {
        Vector3 headPosition = transform.position + Vector3.up * 2;
        Vector3 direction = toPosition - headPosition;
        float distance = direction.magnitude;

        if (distance > distanceLimit)
        {
            return false;
        }
        if (Vector3.Angle(direction, transform.forward) > 60)
        {
            return false;
        }

        // obstacleLayerMask에 해당하는 레이어의 오브젝트만 검사
        if (Physics.Raycast(headPosition, direction, out RaycastHit hit, distance, obstacleLayerMask))
        {
            return false; // 장애물이 있는 경우
        }
        return true; // 장애물이 없는 경우
    }

    public void Move()
    {
        float startDist = Vector3.Distance(transform.position, startPos);
        float endDist = Vector3.Distance(transform.position, endPos);

        if (Vector3.Distance(currentDest, startPos) < 0.01f && startDist < 0.5f)
        {
            currentDest = endPos;
        }
        else if (Vector3.Distance(currentDest, endPos) < 0.01f && endDist < 0.5f)
        {
            currentDest = startPos;
        }
        nmAgent.SetDestination(currentDest);
    }
    void AlertZero()
    {
        initDistance = (initPosition - transform.position).magnitude;
        alertState = 0;
        nmAgent.isStopped = false;
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

    void AlertOne()
    {
        returnToWork = true;
        alertState = 1;
        nmAgent.isStopped = false;
        animator.SetBool("Patrol", true);
        animator.SetBool("Is_Aiming", false);
        nmAgent.SetDestination(playerPosition.position);
        delayCount = 2;
    }

    void AlertTwo()
    {
        alertState = 2;
        nmAgent.isStopped = true;
        animator.SetBool("Is_Aiming", true);
        Vector3 firePosition = transform.position + transform.up * 1.41f + transform.forward * 1.79f + transform.right * 0.21f;
        direction = playerPosition.position - firePosition;
        transform.LookAt(new Vector3(playerPosition.position.x, transform.position.y, playerPosition.position.z));
        if (Time.time - lastAttackTime > 0.5f)
        {
            if (delayCount > 0)
            {
                delayCount--;
                lastAttackTime = Time.time;
            }
            else
            {
                Instantiate(laserPrefab, firePosition, Quaternion.LookRotation(direction));
                lastAttackTime = Time.time;
            }
        }
    }

    // Start is called before the first frame update
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
        delayCount = 2;
        
        enemyHealthManager = GetComponent<EnemyHealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealthManager != null && enemyHealthManager.checkDeath())
        {
            if (nmAgent != null && nmAgent.enabled)
            {
                nmAgent.enabled = false; // Disable the NavMeshAgent on death
            }
            return; // Exit Update if the enemy is dead
        }
        playerPosition = player.transform;
        initDistance = (initPosition - transform.position).magnitude;
        if (IsVisible(playerPosition.position, 12))
        {
            if ((alertState >= 2 && IsVisible(playerPosition.position, 10)) || IsVisible(playerPosition.position, 8))
            {
                AlertTwo();
            }
            else
            {
                AlertOne();
            }
            lastDetectTime = Time.time;
        }
        else if (Time.time - lastDetectTime < 3)
        {
            AlertOne();
        }
        else
        {
            AlertZero();
        }
    }
}