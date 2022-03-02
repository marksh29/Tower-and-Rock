using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hostage : MonoBehaviour
{
    public enum colorState
    {
        Player, Enemy, Neutral, EnemyMove
    }

    public colorState _status;
    [SerializeField] int life;

    [SerializeField] float playerSpeed, enemySpeed, _rotateSpeed;
    float speed;
    public Transform _target;
    Vector3 relativePos;
    public bool move, end, tower;
    [SerializeField] Animator anim;
    string _state;

    [SerializeField] Material[] material;
    [SerializeField] SkinnedMeshRenderer skin;
    [SerializeField] GameObject[] _towers;
    [SerializeField] GameObject destroyPrefab;

    GameObject curTower;

    private void OnEnable()
    {
        //life = 1;
        //curTower = null;
        //SetStateColor("Neutral");
    }
    void Start()
    {
        Sort();
    }
    void Update()
    {
        if(Controll.Instance._state == "Game")
        {
            if (!tower) // && move)
            {
                if ((transform.position - _target.position).sqrMagnitude > 0.2f && _state != "stay")
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    relativePos = new Vector3(_target.position.x, _target.position.y, _target.position.z) - transform.position;
                    Quaternion toRotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.position.x, _target.position.y, _target.position.z), speed * Time.deltaTime);
                }
                else
                    SetState("stay");
            }            

            if (tower)
            {
                relativePos = new Vector3(_target.position.x, _target.position.y, _target.position.z) - transform.position;
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.position.x, _target.position.y, _target.position.z), speed * Time.deltaTime);
                if (!_target.gameObject.activeSelf)
                {
                    SetEnemy();
                }
            }
        }
        else
        {
            SetState("stay");
        }        
    }
  
    public void StartMove(Transform target)
    {
        if(!end)
        {
            move = true;
            _target = target;
        }   
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "TowerFire")
        {
            if(_status == colorState.Enemy)
                coll.gameObject.GetComponent<TowerFire>().AddTarget(gameObject.transform);
            else if(_status == colorState.Player)
                coll.gameObject.GetComponent<TowerFire>().PlayerTarget(gameObject.transform);
        }      
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "TowerFire")
        {
            if (_status == colorState.Enemy)
                coll.gameObject.GetComponent<TowerFire>().RemoveTarget(gameObject.transform);
            else if (_status == colorState.Player)
                coll.gameObject.GetComponent<TowerFire>().RemovePlayerTarget(gameObject.transform);
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Tower" && tower)
        {
            Tower _tw = coll.gameObject.GetComponent<Tower>();
            if(_target == coll.gameObject.transform)
            {
                _tw.Enter(_status.ToString());
                Off();
            }                   
        }
        if (coll.gameObject.tag == "Host" && coll.gameObject.GetComponent<Hostage>()._status == Hostage.colorState.Enemy && _status == colorState.Player)
        {
            coll.gameObject.GetComponent<Hostage>().Destroy();
            Off();
        }
        //if (coll.gameObject.tag == "Player" && _status == colorState.Enemy)
        //{
        //    coll.gameObject.GetComponent<Player>().Damage();
        //    Off();
        //}
    }
    public void SetState(string name)
    {
        if(!end)
        {
            _state = name;
            anim.SetTrigger(name);
            if (name == "stay")
            {
                move = false;
                tower = false;
            }              
        }        
    }
   
    public void MoveToTower(Transform towerPos)
    {
        _target = towerPos;
        tower = true;
        SetState("move");        
    }
    public void SetStateColor(string name)
    {
        switch(name)
        {
            case ("Player"):
                speed = playerSpeed;
                _status = colorState.Player;
                gameObject.layer = 13;
                gameObject.name = "PlayerHost";
                if (transform.parent != null && transform.parent.gameObject.GetComponent<Spawner>() != null)
                {
                    transform.parent.gameObject.GetComponent<Spawner>().RemoveCount(gameObject);
                    transform.parent = null;
                }
                skin.material = material[0];
                transform.GetChild(3).gameObject.tag = "Untagged";
                transform.GetChild(3).gameObject.SetActive(false);

                //transform.position = _target.position;
                break;
            case ("Enemy"):
                speed = enemySpeed;
                gameObject.layer = 8;
                gameObject.name = "EnemyHost";
                _status = colorState.Enemy;
                if (transform.parent!= null && transform.parent.gameObject.GetComponent<Spawner>() != null)
                {
                    transform.parent.gameObject.GetComponent<Spawner>().RemoveCount(gameObject);
                    transform.parent = null;
                }               
                skin.material = material[1];
                transform.GetChild(3).gameObject.tag = "Untagged";
                transform.GetChild(3).gameObject.SetActive(false);
                break;
            case ("Neutral"):
                gameObject.layer = 6;
                _status = colorState.Neutral;
                gameObject.name = "NeutralHost";
                skin.material = material[2];
                transform.GetChild(3).gameObject.tag = "Hostage";
               transform.GetChild(3).gameObject.SetActive(true);
                DropState();
                break;
            case ("EnemyMove"):
                gameObject.layer = 8;
                speed = enemySpeed;                
                gameObject.name = "EnemyMoveHost";
                _status = colorState.Enemy;
                if (transform.parent != null && transform.parent.gameObject.GetComponent<Spawner>() != null)
                {
                    transform.parent.gameObject.GetComponent<Spawner>().RemoveCount(gameObject);
                    transform.parent = null;
                }
                skin.material = material[1];
                transform.GetChild(3).gameObject.tag = "Untagged";
                transform.GetChild(3).gameObject.SetActive(false);

                //transform.position = _target.position;
                break;
        }       
    }
    void DropState()
    {
        move = false;
        tower = false;
        end = false;
        SetState("stay");
    }

    public void SetEnemy()
    {        
        SetStateColor("Enemy");
        transform.GetChild(3).gameObject.SetActive(false);
        SelectTower();
    }

    void Sort()
    {
        _towers = GameObject.FindGameObjectsWithTag("Tower");
        _towers = _towers.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        foreach (GameObject point in _towers) ;      
    }
    void SelectTower()
    {       
        Sort();

        for (int i = 0; i < _towers.Length; i++)
        {
            Tower _tw = _towers[i].GetComponent<Tower>();
            if (_tw._state == Tower.towerState.Neutral || _tw._state == Tower.towerState.Player)
            {
                curTower = _towers[i];
            }           
        }

        if (curTower == null)
            curTower = _towers[0];
        MoveToTower(curTower.transform);
    }
    public void Destroy()
    {
        if (transform.parent != null && transform.parent.gameObject.GetComponent<Spawner>() != null)
        {
            transform.parent.gameObject.GetComponent<Spawner>().RemoveCount(gameObject);
            transform.parent = null;
        }
        GameObject obj = PoolControll.Instance.Spawn("Effect");
        obj.transform.position = transform.position;

        switch (_status)
        {
            case (colorState.Player):
                obj.GetComponent<ParticleSystemRenderer>().material = material[0];
                break;
            case (colorState.Enemy):
                obj.GetComponent<ParticleSystemRenderer>().material = material[1];
                break;
            case (colorState.Neutral):
                obj.GetComponent<ParticleSystemRenderer>().material = material[2];
                break;
        }       
        //Destroy(obj, 1);
        Off();       
    }
   
    private void Off()
    {
        if (_status == colorState.Player)
        {
            Player.Instance?.RemoveHostage(gameObject);
        }
        if (_status == colorState.Enemy)
        {
            Enemy.Instance?.RemoveHostage(gameObject);
        }

        if (Controll.Instance._state == "Game")
        {
            for (int i = 0; i < _towers.Length; i++)
            {
                if (_status == colorState.Player)
                {
                    _towers[i].GetComponent<Tower>().towerFire.RemovePlayerTarget(gameObject.transform);
                }
                if (_status == colorState.Enemy)
                    _towers[i].GetComponent<Tower>().towerFire.RemoveTarget(gameObject.transform);
            }
        }
        DropState();
        gameObject.SetActive(false);
    }
   
    public void LeftStep()
    {
    }
    public void RightStep()
    {
    }
}
