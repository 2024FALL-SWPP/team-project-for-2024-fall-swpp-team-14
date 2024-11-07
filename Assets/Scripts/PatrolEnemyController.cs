using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyController : MonoBehaviour
{
    public float initX, initY, initZ, initYRot;
    public float rangeX, rangeZ, speed;
    Vector3 startPos, endPos, currentDest;
    private Animator animator;

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

    public void move()
    {
        float startDist = Vector3.Distance(transform.position, startPos);
        float endDist = Vector3.Distance(transform.position, endPos);

        if (startDist < 0.1)
        {
            currentDest = endPos;
            transform.LookAt(currentDest);
        }
        else if (endDist < 0.1)
        {
            currentDest = startPos;
            transform.LookAt(currentDest);
        }

        transform.position = Vector3.MoveTowards(transform.position, currentDest, speed * Time.deltaTime);
    }
    void Start()
    {
        startPos = new Vector3(initX, initY, initZ);
        endPos = new Vector3(rangeX, initY, rangeZ);
        currentDest = endPos;

        animator = GetComponent<Animator>();
        animator.SetBool("Patrol", true);
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }
}