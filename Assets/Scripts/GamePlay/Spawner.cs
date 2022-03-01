using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform[] spawnPos;
    public int startSpawn, count, maxCount;  //-- 1.Кол-во хостов при старте 2.Текущее число хостов 3.Максимальное число хостов
    [SerializeField] float spawnTimer; // Время между спавнами
    float timer;
    [SerializeField] List<GameObject> hosts;

    void Start()
    {
        timer = spawnTimer;
        for (int i = 0; i < startSpawn; i++)
        {
            Spawn();
        }
    }
    void Update()
    {
        if(Controll.Instance._state == "Game" && count < maxCount)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = spawnTimer;
                Spawn();
            }
        }
    }
    void Spawn()
    {
        //GameObject obj = Instantiate(prefab, spawnPos[count].position, Quaternion.Euler(0, Random.Range(0, 360), 0), transform) as GameObject;
        GameObject obj = PoolControll.Instance.Spawn("Host");
        obj.GetComponent<Hostage>().SetStateColor("Neutral");
        obj.transform.position = spawnPos[count].position;
        obj.transform.parent = transform;
        hosts.Add(obj);
        count = hosts.Count;
    }
    public void RemoveCount(GameObject obj)
    {
        hosts.Remove(obj);
        count = hosts.Count;
    }
    public void GetAllHost(Enemy _enemy)
    {
        for (int i = count - 1; i >= 0; i--)
        {
            _enemy.AddHost(hosts[i]);            
        }
    }
}
