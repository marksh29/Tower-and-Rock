using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFire : MonoBehaviour
{
    public enum towerState
    {
        Player, Enemy, Neutral
    }
    public towerState _state;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform roof;
    //[SerializeField] List<GameObject> levels; 
    [SerializeField] List<Transform> target, playerTarget;
    float startRadius, addRadius, fireTimer;
    [SerializeField] float timer;
    [SerializeField] bool fireOn;
    int lvl;

    private void Awake()
    {
        startRadius = transform.parent.gameObject.GetComponent<Tower>().startRadius;
        addRadius = transform.parent.gameObject.GetComponent<Tower>().addRadius;
        fireTimer = transform.parent.gameObject.GetComponent<Tower>().fireTimer;
    }
    void Start()
    {        
        timer = 0;
        GetComponent<SphereCollider>().radius = startRadius;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (Controll.Instance._state == "Game" && fireOn)
        {            
            if(timer <= 0)
            {
                if(_state == towerState.Player && target.Count > 0 || _state == towerState.Enemy && playerTarget.Count > 0)
                {
                    timer = fireTimer;
                    Fire();
                }                
            }
        }
    } 
    void Fire()
    {
        //GameObject bull = Instantiate(bulletPrefab, new Vector3(roof.position.x, roof.position.y + 1, roof.position.z), transform.rotation) as GameObject;
        GameObject bull = PoolControll.Instance.Spawn("Bullet");//, new Vector3(roof.position.x, roof.position.y + 1, roof.position.z), transform.rotation) as GameObject;
        bull.transform.position = new Vector3(roof.position.x, roof.position.y + 1, roof.position.z);

        switch (_state)
        {
            case (towerState.Enemy):
                if (playerTarget[0].gameObject.activeSelf)
                {
                    //GameObject bull = Instantiate(bulletPrefab, new Vector3(roof.position.x, roof.position.y + 1, roof.position.z), transform.rotation) as GameObject;
                    bull.GetComponent<Bullet>().SetTarget(playerTarget[0], 1);
                    if (playerTarget[0].gameObject.GetComponent<Player>() == null)// && playerTarget[0].gameObject.GetComponent<Player>().life > 1)
                    {
                        RemovePlayerTarget(playerTarget[0]);
                    }
                }
                break;
            case (towerState.Player):
                //GameObject bull1 = Instantiate(bulletPrefab, new Vector3(roof.position.x, roof.position.y + 1, roof.position.z), transform.rotation) as GameObject;
                bull.GetComponent<Bullet>().SetTarget(target[0], 0);
                RemoveTarget(target[0]);
                break;
        }     
    }

    public void SetFireState(bool fire, int count)
    {
        lvl = count;
        fireOn = fire;
        GetComponent<SphereCollider>().radius = startRadius + (addRadius * count);
    }

    public void AddTarget(Transform obj)
    {
        target.Add(obj);
    }
    public void PlayerTarget(Transform obj)
    {
        playerTarget.Add(obj);
    }

    public void RemoveTarget(Transform obj)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if(obj == target[i])
                target.Remove(target[i]);
        }        
    }
    public void RemovePlayerTarget(Transform obj)
    {
        for (int i = 0; i < playerTarget.Count; i++)
        {
            if (obj == playerTarget[i])
            {
                playerTarget.Remove(playerTarget[i]);
            }                
        }
    }

    public void SetState(string name)
    {
        switch (name)
        {
            case ("Player"):
                _state = towerState.Player;
                break;
            case ("Enemy"):
                _state = towerState.Enemy;
                break;
            case ("Neutral"):
                _state = towerState.Neutral;
                break;
        }
    }
}
