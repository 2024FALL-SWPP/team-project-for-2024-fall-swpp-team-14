using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private float initX, initY, initZ, initYRot;
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
}

public class WorkingEnemy : Enemy
{
    private float rangeX, rangeZ, speed;
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
}

public class StandingEnemy : Enemy
{
    private float speed;
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public float getSpeed()
    {
        return this.speed;
    }
}

public class PatrolEnemy : Enemy
{
    private float rangeX, rangeZ, speed;
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
}

public class EnemyBuilder<TBuilder, TEnemy> // Using builder pattern to generate enemies.
    where TBuilder : EnemyBuilder<TBuilder, TEnemy>
    where TEnemy : Enemy, new()
{
    protected TEnemy enemy;
    public EnemyBuilder()
    {
        this.enemy = new TEnemy();
    }
    public TBuilder InitX(float initX)
    {
        this.enemy.setInitX(initX);
        return (TBuilder)this;
    }
    public TBuilder InitY(float initY)
    {
        this.enemy.setInitY(initY);
        return (TBuilder)this;
    }
    public TBuilder InitZ(float initZ)
    {
        this.enemy.setInitZ(initZ);
        return (TBuilder)this;
    }
    public TBuilder InitYRot(float initYRot)
    {
        this.enemy.setInitYRot(initYRot);
        return (TBuilder)this;
    }
    public TEnemy GetEnemy()
    {
        return this.enemy;
    }
}

public class EnemyBuilder : EnemyBuilder<EnemyBuilder, Enemy>
{
}

public class WorkingEnemyBuilder : EnemyBuilder<WorkingEnemyBuilder, WorkingEnemy>
{
    public WorkingEnemyBuilder RangeX(float rangeX)
    {
        this.enemy.setRangeX(rangeX);
        return this;
    }
    public WorkingEnemyBuilder RangeZ(float rangeZ)
    {
        this.enemy.setRangeZ(rangeZ);
        return this;
    }
    public WorkingEnemyBuilder Speed(float speed)
    {
        this.enemy.setSpeed(speed);
        return this;
    }
}

public class StandingEnemyBuilder : EnemyBuilder<StandingEnemyBuilder, StandingEnemy>
{
    public StandingEnemyBuilder Speed(float speed)
    {
        enemy.setSpeed(speed);
        return this;
    }
}

public class PatrolEnemyBuilder : EnemyBuilder<PatrolEnemyBuilder, PatrolEnemy>
{
    public PatrolEnemyBuilder RangeX(float rangeX)
    {
        enemy.setRangeX(rangeX);
        return this;
    }
    public PatrolEnemyBuilder RangeZ(float rangeZ)
    {
        enemy.setRangeZ(rangeZ);
        return this;
    }
    public PatrolEnemyBuilder Speed(float speed)
    {
        enemy.setSpeed(speed);
        return this;
    }
}

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject workingEnemyPrefab;
    public GameObject standingEnemyPrefab;
    public GameObject patrolEnemyPrefab;
   public void EnemySetter(float initX, float initY, float initZ, float initYRot)
    {
        EnemyBuilder enemyBuilder = new EnemyBuilder();
        Enemy enemy = enemyBuilder.InitX(initX).InitY(initY).InitZ(initZ).InitYRot(initYRot).GetEnemy();
        GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(enemy.getInitX(), enemy.getInitY(), enemy.getInitZ()), Quaternion.Euler(0, enemy.getInitYRot(), 0));
    }
    public void WorkingEnemySetter(float initX, float initY, float initZ, float initYRot, float rangeX, float rangeZ, float speed)
    {
        WorkingEnemyBuilder enemyBuilder = new WorkingEnemyBuilder();
        WorkingEnemy enemy = enemyBuilder.InitX(initX).InitY(initY).InitZ(initZ).InitYRot(initYRot).RangeX(rangeX).RangeZ(rangeZ).Speed(speed).GetEnemy();
        GameObject enemyInstance = Instantiate(workingEnemyPrefab, new Vector3(enemy.getInitX(), enemy.getInitY(), enemy.getInitZ()), Quaternion.Euler(0, enemy.getInitYRot(), 0));
        WorkingEnemyController enemyController = enemyInstance.GetComponent<WorkingEnemyController>();
        enemyController.setInitX(enemy.getInitX());
        enemyController.setInitY(enemy.getInitY());
        enemyController.setInitZ(enemy.getInitZ()); 
        enemyController.setInitYRot(enemy.getInitYRot());
        enemyController.setRangeX(enemy.getRangeX());
        enemyController.setRangeZ(enemy.getRangeZ());
        enemyController.setSpeed(enemy.getSpeed());
    }
    public void StandingEnemySetter(float initX, float initY, float initZ, float initYRot, float speed)
    {
        StandingEnemyBuilder enemyBuilder = new StandingEnemyBuilder();
        StandingEnemy enemy = enemyBuilder.InitX(initX).InitY(initY).InitZ(initZ).InitYRot(initYRot).Speed(speed).GetEnemy();
        GameObject enemyInstance = Instantiate(standingEnemyPrefab, new Vector3(enemy.getInitX(), enemy.getInitY(), enemy.getInitZ()), Quaternion.Euler(0, enemy.getInitYRot(), 0));
        StandingEnemyController enemyController = enemyInstance.GetComponent<StandingEnemyController>();
        enemyController.setInitX(enemy.getInitX());
        enemyController.setInitY(enemy.getInitY());
        enemyController.setInitZ(enemy.getInitZ());
        enemyController.setInitYRot(enemy.getInitYRot());
        enemyController.setSpeed(enemy.getSpeed());
    }
    public void PatrolEnemySetter(float initX, float initY, float initZ, float initYRot, float rangeX, float rangeZ, float speed)
    {
        PatrolEnemyBuilder enemyBuilder = new PatrolEnemyBuilder();
        PatrolEnemy enemy = enemyBuilder.InitX(initX).InitY(initY).InitZ(initZ).InitYRot(initYRot).RangeX(rangeX).RangeZ(rangeZ).Speed(speed).GetEnemy();
        GameObject enemyInstance = Instantiate(patrolEnemyPrefab, new Vector3(enemy.getInitX(), enemy.getInitY(), enemy.getInitZ()), Quaternion.Euler(0, enemy.getInitYRot(), 0));
        PatrolEnemyController enemyController = enemyInstance.GetComponent<PatrolEnemyController>();
        enemyController.setInitX(enemy.getInitX());
        enemyController.setInitY(enemy.getInitY());
        enemyController.setInitZ(enemy.getInitZ());
        enemyController.setInitYRot(enemy.getInitYRot());
        enemyController.setRangeX(enemy.getRangeX());
        enemyController.setRangeZ(enemy.getRangeZ());
        enemyController.setSpeed(enemy.getSpeed());
    }
    // Start is called before the first frame update
    void Start()
    {
        //EnemySetter(60.82f, 2.1f, -5.93f, 90f);
        //WorkingEnemySetter(60.82f, 2.1f, -5.93f, 90f, 65.82f, -5.93f, 2f);

        PatrolEnemySetter(29.37f, 9.8f, -6.41f, 90f, 56.17f, -6.41f, 2f);
        PatrolEnemySetter(56.17f, 9.8f, -19.9f, -90f, 29.37f, -19.9f, 2f);
        StandingEnemySetter(59.82f, 2.09182f, -5.931f, 90f, 2f);
        StandingEnemySetter(59.82f, 1.908182f, -9.72f, 90f, 2f);
        PatrolEnemySetter(54.637f, 3.385209f, -29.76f, 0f, 54.637f, -19.76f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
