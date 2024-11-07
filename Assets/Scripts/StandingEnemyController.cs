using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingEnemyController : MonoBehaviour
{
    public float initX, initY, initZ, initYRot;
    public float rangeX, rangeZ, speed;
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
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public float getSpeed()
    {
        return this.speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
