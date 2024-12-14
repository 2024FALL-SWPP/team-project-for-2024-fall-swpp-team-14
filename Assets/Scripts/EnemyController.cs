using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    public GameObject player;
    public Transform playerPosition;
    public float initX, initY, initZ, initYRot;
    public float speed;
    protected Animator animator;
    protected Vector3 direction;
    protected LayerMask obstacleLayerMask = 1 << 6;
    protected UnityEngine.AI.NavMeshAgent nmAgent;
    protected float lastDetectTime;
    protected float lastAttackTime;
    protected Vector3 initPosition;
    public float initDistance;
    public GameObject laserPrefab;
    public int alertState = 0;
    protected int delayCount = 2;
    protected MainMapManager mainMapManager;
    protected EnemyHealthManager enemyHealthManager;
    protected float AimAngle;
    protected Vector3 firePosition;
    protected bool returnToWork = false;
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

    protected bool IsVisible(Vector3 toPosition, int distanceLimit)
    {
        Vector3 headPosition = transform.position + Vector3.up * 2;
        Vector3 direction = toPosition - headPosition;
        float distance = direction.magnitude;

        AimAngle = 90.0f - Vector3.Angle(direction, transform.up);
        animator.SetFloat("Aim_Angle", AimAngle);

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
    protected abstract void AlertZero();
    protected void AlertOne()
    {
        if (alertState != 3)
        {
            alertState = 1;
        }
        returnToWork = true;
        nmAgent.isStopped = false;
        animator.SetBool("Patrol", true);
        animator.SetBool("Is_Aiming", false);
        nmAgent.SetDestination(playerPosition.position);
        delayCount = 2;
    }
    protected void AlertTwo()
    {
        if (alertState != 3)
        {
            alertState = 2;
        }
        nmAgent.isStopped = true;
        animator.SetBool("Is_Aiming", true);

        if (AimAngle > 15)
        {
            firePosition = transform.position + transform.up * 2.3f + transform.forward * 1.41f + transform.right * 0.35f;
        }
        else if (AimAngle < 15 && AimAngle > -25)
        {
            firePosition = transform.position + transform.up * 1.41f + transform.forward * 1.79f + transform.right * 0.21f;
        }
        else if (AimAngle < -25)
        {
            firePosition = transform.position + transform.up * 0.35f + transform.forward * 1.3f + transform.right * (-0.09f);
        }

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
    protected void AlertThree()
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

    protected void Update()
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
            IsVisible(playerPosition.position, 100);    // To call animator.SetFloat
            AlertThree();
        }
        else if (IsVisible(playerPosition.position, 12))
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
