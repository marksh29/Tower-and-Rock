using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float spawnTimer;
    [SerializeField] int spawnCount;
    [SerializeField] GameObject prefab;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] bool oneSpawn;
    float timer;
    bool end;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    void Start()
    {
        timer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(Controll.Instance._state == "Game" && !end)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                end = oneSpawn;
                timer = spawnTimer;
                Spawn();                             
            }
        }
    }
    void Spawn()
    {
        int spCount = spawnCount < spawnPos.Length - 1 ? spawnCount : spawnPos.Length;
        for (int i = 0; i < spawnCount; i++)
        {
            //GameObject obj = Instantiate(prefab, spawnPos[i].position, spawnPos[i].rotation) as GameObject;
            GameObject obj = PoolControll.Instance.Spawn("Host");
            obj.transform.position = spawnPos[i].position;
            obj.transform.parent = transform;
            obj.GetComponent<Hostage>().SetEnemy();
        }        
    }
}
