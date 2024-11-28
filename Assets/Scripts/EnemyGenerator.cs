using System.Collections;
using System.Collections.Generic;
using System;
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
    public GameObject enemyLaserPrefab;
    private MainMapManager mainMapManager;
    public List<GameObject> enemyList = new List<GameObject>();
    private List<GameObject> totalEnemyList = new List<GameObject>();

    public int maxAlert;
    private int alertState = 0;

    public GameObject EnemySetter(float initX, float initY, float initZ, float initYRot)
    {
        EnemyBuilder enemyBuilder = new EnemyBuilder();
        Enemy enemy = enemyBuilder.InitX(initX).InitY(initY).InitZ(initZ).InitYRot(initYRot).GetEnemy();
        GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(enemy.getInitX(), enemy.getInitY(), enemy.getInitZ()), Quaternion.Euler(0, enemy.getInitYRot(), 0));
        return enemyInstance;
    }
    public GameObject WorkingEnemySetter(float initX, float initY, float initZ, float initYRot, float rangeX, float rangeZ, float speed)
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
        enemyController.setLaserPrefab(enemyLaserPrefab);
        return enemyInstance;
    }
    public GameObject StandingEnemySetter(float initX, float initY, float initZ, float initYRot, float speed)
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
        enemyController.setLaserPrefab(enemyLaserPrefab);
        return enemyInstance;
    }
    public GameObject PatrolEnemySetter(float initX, float initY, float initZ, float initYRot, float rangeX, float rangeZ, float speed)
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
        enemyController.setLaserPrefab(enemyLaserPrefab);
        return enemyInstance;
    }
    // Start is called before the first frame update
    void Start()
    {
        maxAlert = 0;
        mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        //EnemySetter(60.82f, 2.1f, -5.93f, 90f);
        //WorkingEnemySetter(60.82f, 2.1f, -5.93f, 90f, 65.82f, -5.93f, 2f);

        // 1층 창고 경비병 하나
        totalEnemyList.Add(PatrolEnemySetter(54.637f, 3.385209f, -29.76f, 0f, 54.637f, -19.76f, 2f));

        // 1층 정문 경비병 둘
        totalEnemyList.Add(StandingEnemySetter(59.82f, 2.09182f, -5.931f, 90f, 2f));
        totalEnemyList.Add(StandingEnemySetter(59.82f, 1.908182f, -9.72f, 90f, 2f));

        // 1층 후문 경비병 하나
        totalEnemyList.Add(PatrolEnemySetter(25.63f, 2.0919f, -21.96f, 0f, 25.63f, -17.4f, 2f));

        // 1층 검문소 경비병 넷
        totalEnemyList.Add(StandingEnemySetter(56.36145f, 3.405238f, -6f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(53.36145f, 3.405238f, -6f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(50.36145f, 3.405238f, -6f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(47.36145f, 3.405238f, -6f, 180f, 2f));

        // 1층 내부 중앙 복도 경비병 둘
        totalEnemyList.Add(StandingEnemySetter(50.6f, 3.385209f, -17.72f, -120f, 2f));
        totalEnemyList.Add(StandingEnemySetter(50.6f, 3.385209f, -22.13f, -60f, 2f));

        // 1층 광장 노동자 셋
        totalEnemyList.Add(WorkingEnemySetter(35.19f, 3.38521f, -36.33f, 90f, 41.19f, -36.33f, 2f));
        totalEnemyList.Add(WorkingEnemySetter(38.17f, 3.38521f, -33.45f, 90f, 48.17f, -33.45f, 2f));
        totalEnemyList.Add(WorkingEnemySetter(53.28f, 3.38521f, -36.33f, -90f, 46.28f, -36.33f, 2f));

        // 계단 위 경비병 하나
        totalEnemyList.Add(StandingEnemySetter(31.08f, 8.28f, -33.635f, 180f, 2f));

        // 2층 진입지 경비병 여섯
        totalEnemyList.Add(StandingEnemySetter(35.3f, 9.8f, -27.22f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(38.3f, 9.8f, -27.22f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(41.3f, 9.8f, -27.22f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(44.3f, 9.8f, -27.22f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(47.3f, 9.8f, -27.22f, 180f, 2f));
        totalEnemyList.Add(StandingEnemySetter(50.3f, 9.8f, -27.22f, 180f, 2f));
        
        // 2층 서버실 순찰조 경비병 둘
        totalEnemyList.Add(PatrolEnemySetter(29.37f, 9.8f, -6.41f, 90f, 56.17f, -6.41f, 2f));
        totalEnemyList.Add(PatrolEnemySetter(56.17f, 9.8f, -19.9f, -90f, 29.37f, -19.9f, 2f));

        // 2층 서버실 경비병 하나
        totalEnemyList.Add(StandingEnemySetter(38.99f, 9.8f, -10.131f, 0f, 2f));

        // 2층 서버실 옆 비활성화된 경비병 여섯
        enemyList.Add(EnemySetter(46.332f, 9.8f, -12.508f, 90f));
        enemyList.Add(EnemySetter(46.332f, 9.8f, -14.908f, 90f));
        enemyList.Add(EnemySetter(47.489f, 9.8f, -16.33f, 0f));
        enemyList.Add(EnemySetter(49.91f, 9.8f, -16.33f, 0f));
        enemyList.Add(EnemySetter(51.221f, 9.8f, -14.897f, -90f));
        enemyList.Add(EnemySetter(51.221f, 9.8f, -12.508f, -90f));
    }

    // Update is called once per frame
    void Update()
    {
        if (mainMapManager.isServerActivated)
        {
            for(int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i] == null)
                {
                    continue;
                }

                Vector3 tempPos = enemyList[i].transform.position;
                float tempYRot = enemyList[i].transform.rotation.y;

                Destroy(enemyList[i]);
                totalEnemyList.Add(StandingEnemySetter(tempPos.x, tempPos.y, tempPos.z, tempYRot, 2f));
            }
            enemyList = new List<GameObject>();
        }

        maxAlert = 0;
        foreach (GameObject enemy in totalEnemyList)
        {
            if (enemy == null)
            {
                continue;
            }
            alertState = 0;
            if (enemy.GetComponent<StandingEnemyController>() != null)
            {
                alertState = enemy.GetComponent<StandingEnemyController>().alertState;
            }
            else if (enemy.GetComponent<WorkingEnemyController>() != null)
            {
                alertState = enemy.GetComponent<WorkingEnemyController>().alertState;
            }
            else if (enemy.GetComponent<PatrolEnemyController>() != null)
            {
                alertState = enemy.GetComponent<PatrolEnemyController>().alertState;
            }

            maxAlert = Math.Max(maxAlert, alertState);
        }
    }
}
