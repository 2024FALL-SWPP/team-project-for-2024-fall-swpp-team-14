using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingEnemyController : MonoBehaviour
{
    public GameObject player;
    public Transform playerPosition;
    public float initX, initY, initZ, initYRot;
    public float speed;
    private Animator animator;
    private Vector3 direction;
    private LayerMask obstacleLayerMask = 1 << 6;
    private UnityEngine.AI.NavMeshAgent nmAgent;
    private float lastDetectTime;
    private float lastAttackTime;
    private Vector3 initPosition;
    public float initDistance;
    public GameObject laserPrefab;
    public int alertState = 0;
    private int delayCount = 2;
    private MainMapManager mainMapManager;
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
    public void setSpeed(float speed)
    {
        this.speed = speed;
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

    void AlertZero()
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

    void AlertOne()
    {
        if (alertState != 3)
        {
            alertState = 1;
        }
        nmAgent.isStopped = false;
        animator.SetBool("Patrol", true);
        animator.SetBool("Is_Aiming", false);
        nmAgent.SetDestination(playerPosition.position);
        delayCount = 2;
    }

    void AlertTwo()
    {
        if (alertState != 3)
        {
            alertState = 2;
        }
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

    void AlertThree()
    {
        alertState = 3;
        playerPosition = player.transform;

        if (IsVisible(playerPosition.position,10)) //if ((alertState >= 2 && IsVisible(playerPosition.position, 10)) || IsVisible(playerPosition.position, 8))
        {
            AlertTwo();
        }
        else
        {
            AlertOne();
        }
    }

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

        if (mainMapManager != null && mainMapManager.isServerActivated)
        {
            AlertThree();
        }
        else if (IsVisible(playerPosition.position, 12))
        {
            Debug.Log("Player is visible");
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