using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyControll : MonoBehaviour
{
    public static EnemyControll Instance;
    public enum stateEnemyType
    {
        AttackPlayer, AttackPlayerTower, AttackNeutralTower, DefenceTower, MoveToPosition, GetHost, GetHomeHost, Wait
    }
    [SerializeField] Enemy enemy;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] stateEnemyType _state;
    [SerializeField] float waitTimer;
    [SerializeField] bool move;

    [Header("----------Targets---------")]
    [SerializeField] Transform curTarget;
    [SerializeField] Player player;
    [SerializeField] GameObject[] movePositions;
    [SerializeField] GameObject[] towers;
    [SerializeField] GameObject[] spawners;
    [SerializeField] GameObject[] homeSpawners;
    Transform oldTarget;

    [Header("----------Chanse----------")]
    [SerializeField] int playerAttack;
    [SerializeField] int playerTowerAttack;
    [SerializeField] int neutralTowerAttack;
    [SerializeField] int buildTower;
    [SerializeField] int moveToPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        movePositions = GameObject.FindGameObjectsWithTag("MovePosition");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        towers = GameObject.FindGameObjectsWithTag("Tower");
        spawners = GameObject.FindGameObjectsWithTag("Spawner");

    }
    private void Start()
    {
    }
    public void StartEnemy()
    {
        SetState(stateEnemyType.Wait, 1);
    }

    void Update()
    {
        if (move && curTarget != null && Controll.Instance._state == "Game")
        {
            agent.destination = curTarget.position;
        }
    }
    public void SetState(stateEnemyType type, float waitTime)
    {
        _state = type;        
        switch (_state)
        {
            case (stateEnemyType.AttackPlayer):
                AttackPlayerSort();
                enemy.MoveToTarget(curTarget);
                break;
            case (stateEnemyType.AttackPlayerTower):
                AttackTowerSort();
                enemy.MoveToTarget(curTarget);
                break;
            case (stateEnemyType.AttackNeutralTower):
                NeutralTowerSort();
                enemy.MoveToTarget(curTarget);
                break;
            case (stateEnemyType.DefenceTower):
                DefenceTowerSort();
                enemy.MoveToTarget(curTarget);
                break;
            //case (stateEnemyType.GetHost):
            //    SpawnerSort();
            //    break;
            //case (stateEnemyType.GetHomeHost):
            //    HomeSpawnerSort();
            //    break;
            case (stateEnemyType.MoveToPosition):
                MovePositionSort();
                break;
            case (stateEnemyType.Wait):
                move = false;
                enemy.SetAnimation("stay");
                if(Controll.Instance._state == "Game")
                    StartCoroutine(Wait(waitTime));                
                break;
        }
    }
    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        //int random = Random.Range(0, 100);
        //if (random <= playerAttack)
        //{
        //    if (enemy._hostageList.Count > player._hostageList.Count)
        //    {
        //        curTarget = player.gameObject.transform;
        //        SetState(stateEnemyType.AttackPlayer, 0);
        //    }
        //    //else
        //    //    SetState(stateEnemyType.GetHomeHost, 0);
        //}
        //else if (random > playerAttack && random <= playerTowerAttack)
        //{
        //    SetState(stateEnemyType.AttackPlayerTower, 0);
        //}
        //else if (random > playerTowerAttack && random <= neutralTowerAttack)
        //{
        //    SetState(stateEnemyType.AttackNeutralTower, 0);
        //}
        //else if (random > neutralTowerAttack && random <= buildTower)
        //{
        //    SetState(stateEnemyType.DefenceTower, 0);
        //}
        //else if (random > buildTower && random <= moveToPosition)
        //{
        //    SetState(stateEnemyType.MoveToPosition, 0);
        //}
        if (enemy._hostageList.Count < enemy.mimHostToAttack)
        {
            //SetState(stateEnemyType.GetHost, 0);
            SetState(stateEnemyType.MoveToPosition, 0);
        }
        else
        {
            int random = Random.Range(0, 100);
            if (random <= playerAttack)
            {
                if (enemy._hostageList.Count > player._hostageList.Count)
                {
                    curTarget = player.gameObject.transform;
                    SetState(stateEnemyType.AttackPlayer, 0);
                }
                else
                    SetState(stateEnemyType.MoveToPosition, 0);
                //SetState(stateEnemyType.GetHomeHost, 0);
            }
            else if (random > playerAttack && random <= playerTowerAttack)
            {
                SetState(stateEnemyType.AttackPlayerTower, 0);
            }
            else if (random > playerTowerAttack && random <= neutralTowerAttack)
            {
                SetState(stateEnemyType.AttackNeutralTower, 0);
            }
            else if (random > neutralTowerAttack && random <= buildTower)
            {
                SetState(stateEnemyType.DefenceTower, 0);
            }
        }
    }

    void AttackPlayerSort()
    {
        curTarget = GameObject.FindGameObjectWithTag("Player").transform;
        StartMove(curTarget);
    }
    void AttackTowerSort()
    {
        towers = towers.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        foreach (GameObject point in towers) ;

        List<GameObject> playerTower = new List<GameObject>();
        for (int  i = 0;  i < towers.Length;  i++)
        {
            if(towers[i].GetComponent<Tower>()._state == Tower.towerState.Player)
            {
                playerTower.Add(towers[i]);
                if (curTarget.gameObject.tag != "Tower")
                    curTarget = towers[i].transform;
                else
                {
                   if(towers[i].GetComponent<Tower>().count < curTarget.gameObject.GetComponent<Tower>().count)
                        curTarget = towers[i].transform;
                }
            }
        }
        //--- добавить выборку по кол-ву внутри ---//
        //if (playerTower.Count > 0)
        //{
        //    curTarget = playerTower[0].transform;
        //    StartMove(curTarget);
        //}
        if (playerTower.Count == 0)
        {
            NeutralTowerSort();
        } 
        else
            StartMove(curTarget);
    }
    void NeutralTowerSort()
    {
        towers = towers.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        foreach (GameObject point in towers) ;

        List<GameObject> neutralTower = new List<GameObject>();
        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].GetComponent<Tower>()._state == Tower.towerState.Neutral)
            {
                neutralTower.Add(towers[i]);
            }
        }
        if(neutralTower.Count > 0)
        {
            curTarget = neutralTower[0].transform;
            StartMove(curTarget);
        }
        else
        {
            AttackTowerSort();
        }
        //--- добавить выборку по кол-ву внутри ---//
    }
    void DefenceTowerSort()
    {
        towers = towers.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        foreach (GameObject point in towers) ;

        List<GameObject> defenceTower = new List<GameObject>();
        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].GetComponent<Tower>()._state == Tower.towerState.Enemy && towers[i].GetComponent<Tower>().count < towers[i].GetComponent<Tower>().maxCount)
            {
                defenceTower.Add(towers[i]);
            }
        }
        //--- добавить выборку по кол-ву внутри ---//
        if(defenceTower.Count > 0)
        {
            curTarget = defenceTower[0].transform;
            StartMove(curTarget);
        }
        else
        {
            AttackTowerSort();
        }       
    }

    //void SpawnerSort()
    //{
    //    spawners = spawners.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
    //    foreach (GameObject point in spawners) ;

    //    curTarget = spawners[0].transform;
    //    if (spawners[0].GetComponent<Spawner>().count < spawners[1].GetComponent<Spawner>().count || curTarget == oldTarget) // -- добавить нужную разницу в кол-во хостов
    //        curTarget = spawners[1].transform;

    //    if(spawners.Length > 2)
    //    {
    //        for (int i = 2; i < spawners.Length; i++)
    //        {
    //            if (spawners[i - 2].GetComponent<Spawner>().count + spawners[i - 1].GetComponent<Spawner>().count < spawners[i].GetComponent<Spawner>().count / 3)
    //            {
    //                curTarget = spawners[i].transform;
    //            }
    //        }
    //    }
    //    StartMove(curTarget);
    //}
    //void HomeSpawnerSort()
    //{
    //    homeSpawners = homeSpawners.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
    //    foreach (GameObject point in homeSpawners);

    //    curTarget = homeSpawners[0].transform;
    //    if (homeSpawners[0].GetComponent<Spawner>().count < homeSpawners[1].GetComponent<Spawner>().count || curTarget == oldTarget)
    //        curTarget = homeSpawners[1].transform;
    //    StartMove(curTarget);
    //}
    void MovePositionSort()
    {
        //movePositions = movePositions.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        //foreach (GameObject point in movePositions) ;
        curTarget = movePositions[Random.Range(0, movePositions.Length)].transform;       
        StartMove(curTarget);
    }

    void StartMove(Transform targ)
    {
        move = true;
        oldTarget = targ;
        enemy.SetAnimation("move");
    }
}
