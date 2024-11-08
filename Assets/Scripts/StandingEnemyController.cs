using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingEnemyController : MonoBehaviour
{
    public GameObject player;
    public Transform playerPosition;
    public float initX, initY, initZ, initYRot;
    public float rangeX, rangeZ, speed;
    private Animator animator;
    private Vector3 direction;
    private LayerMask obstacleLayerMask = 1 << 6;

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

    bool IsVisible(Vector3 toPosition)
    {
        Vector3 headPosition = transform.position + Vector3.up * 2;
        Vector3 direction = toPosition - headPosition;
        float distance = direction.magnitude;

        if (distance > 12)
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform;
        if (IsVisible(playerPosition.position))
        {
            direction = playerPosition.position - transform.position;
            direction.y = 0;
            if (direction.magnitude >= 0.5f)
            {
                animator.SetBool("Patrol", true);
                transform.rotation = Quaternion.LookRotation(direction);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            Debug.Log("Player is visible from StandingEnemy.");
        }
        else
        {
            animator.SetBool("Patrol", false);
            Debug.Log("Player is not visible from StandingEnemy.");
        }
    }
}