using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour
{
    public enum gameplayType
    {
        Line, Big
    }
    public static Enemy Instance;
    [SerializeField] EnemyControll enemyControll;
    public gameplayType _gameplay;       
    bool move;

    [Header("Line")]
    [SerializeField] int _lineWidth;
    public int mimHostToAttack;
    public List<GameObject> _hostageList;
    [SerializeField] Transform[] _hostPos;
    Vector3 oldPositions;
    [Header("Big")]
    [SerializeField] int life;
    [SerializeField] float addScale;

    [Header("Other")]
    [SerializeField] Animator anim;
    [SerializeField] TextMeshPro hostCountText;

    [SerializeField] GameObject effect;

    private void Awake()
    {
        SetAnimation("stay");

        life = 1;
        HostText();

        _lineWidth = _lineWidth > _hostPos.Length ? _hostPos.Length : _lineWidth;
        if (Instance == null) Instance = this;
      
    }
    private void Start()
    {
        SetAnimation("stay");
    }

    public void SetAnimation(string name)
    {
        switch (name)
        {
            case "stay":
                anim.SetTrigger("stay");
                HostageState(false);
                break;
            case "move":
                anim.SetTrigger("move");
                HostageState(true);
                break;
        }
    }

    private void LateUpdate()
    {
        SetMovePosition();
    }
    void SetMovePosition()
    {
        oldPositions = _hostPos[0].position;
        for (int i = 0; i < _hostageList.Count; i++)
        {
            if (i < _lineWidth)
            {
                _hostageList[i].GetComponent<Hostage>().StartMove(_hostPos[i]);
            }
            else
            {
                int ct = i - _lineWidth;
                _hostageList[i].GetComponent<Hostage>().StartMove(_hostageList[ct].transform.GetChild(0));
            }
        }
        //if (_hostPos[0].position != oldPositions && _hostageList.Count > 0)
        //{            
        //}
    }

    private void OnTriggerEnter(Collider coll)
    {       
        if (coll.gameObject.tag == "Spawner")
        {
            coll.gameObject.GetComponent<Spawner>().GetAllHost(this);
            enemyControll.SetState(EnemyControll.stateEnemyType.Wait, 1f);
        }
        if (coll.gameObject.tag == "Tower")
        {
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    int id = coll.gameObject.GetComponent<Tower>().FullCount("Enemy", _hostageList.Count);
                    MoveToTower(coll.gameObject.transform, id);
                    enemyControll.SetState(EnemyControll.stateEnemyType.Wait, 0);
                    break;
                case (gameplayType.Big):
                    //int id2 = coll.gameObject.GetComponent<Tower>().FullCount("Enemy", life - 1);
                    //float time = 0.1f;
                    //StartCoroutine(RemoveBoostScale(time, id2));
                    //StartCoroutine(coll.gameObject.GetComponent<Tower>().ScaleAdd(time, id2));
                    break;
            }
        }
        if (coll.gameObject.tag == "Money")
        {
            coll.gameObject.GetComponent<Money>().MoveOn(transform);
        }

        //if (coll.gameObject.tag == "Hostage")
        //{
        //    switch (_gameplay)
        //    {
        //        case (gameplayType.Line):
        //            AddHost(coll.gameObject.transform.parent.gameObject);
        //            break;
        //        case (gameplayType.Big):
        //            coll.gameObject.transform.parent.gameObject.GetComponent<Hostage>().Destroy();
        //            AddScale(1);
        //            break;
        //    }
        //}
        //if (coll.gameObject.tag == "Boost")
        //{
        //    switch (_gameplay)
        //    {
        //        case (gameplayType.Line):
        //            coll.gameObject.GetComponent<Boost>().SetBoost(_hostageList.Count);
        //            break;
        //        case (gameplayType.Big):
        //            coll.gameObject.GetComponent<Boost>().SetScale(life);
        //            break;
        //    }
        //}
    }
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Host" && coll.gameObject.GetComponent<Hostage>()._status == Hostage.colorState.Player)
        {
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    if (_hostageList.Count > 0)
                    {
                        RemoveHosts(1);
                        coll.gameObject.GetComponent<Hostage>().Destroy();
                    }
                    else
                    {
                        Damage();
                    }
                    break;
                case (gameplayType.Big):
                    coll.gameObject.GetComponent<Hostage>().Destroy();
                    RemoveScale();
                    break;
            }
        }
        if (coll.gameObject.tag == "Player")
        {
            if (_hostageList.Count > 0)
            {
                MoveToTower(GameObject.FindGameObjectWithTag("Player").transform, _hostageList.Count);
                enemyControll.SetState(EnemyControll.stateEnemyType.GetHost, 0);
            }
            else
            {
                Damage();
            }
        }
    }
    public void AddHost(GameObject obj)
    {
        _hostageList.Add(obj);
        SetMovePosition();
        obj.GetComponent<Hostage>().SetState("move");
        obj.GetComponent<Hostage>().SetStateColor("EnemyMove");
        HostText();
    }

    public void RemoveHosts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _hostageList[_hostageList.Count - 1].SetActive(false);
            RemoveHostage(_hostageList[_hostageList.Count - 1]);
        }
    }
    public void RemoveHostage(GameObject obj)
    {

        for (int i = 0; i < _hostageList.Count; i++)
        {
            if (obj == _hostageList[i])
            {
                _hostageList.Remove(_hostageList[i]);
                SetMovePosition();
                HostText();
            }                
        }       
    }

    void AddScale()
    {
        life++;        
        transform.localScale += new Vector3(addScale, addScale, addScale);
        HostText();
    }
    public void RemoveScale()
    {
        life--;
        transform.localScale -= new Vector3(addScale, addScale, addScale);
        HostText();
        //if (life <= 0)
        //    Damage();
    }
    public void SetScale(int id)
    {
        life = id;
        float scl = 1f + (float)id * addScale;
        transform.localScale = new Vector3(scl,scl, scl);
        HostText();
    } 

    public IEnumerator RemoveBoostScale(float time, int id)
    {
        for (int  i = 0;  i < id;  i++)
        {
            RemoveScale();
            yield return new WaitForSeconds(time);
        }        
    }

    public void HostText()
    {
        switch (_gameplay)
        {
            case (gameplayType.Line):
                hostCountText.gameObject.transform.parent.gameObject.SetActive(_hostageList.Count > 0 ? true : false);
                hostCountText.text = _hostageList.Count.ToString();
                break;
            case (gameplayType.Big):
                hostCountText.gameObject.transform.parent.gameObject.SetActive(life < 50 ? (life > 1 ? true : false) : false);
                hostCountText.text = (life - 1).ToString();
                break;
        }
    }
    
    public void HostageState(bool _move)
    {
        move = _move;      
        for (int i = 0; i < _hostageList.Count; i++)
        {
            _hostageList[i].GetComponent<Hostage>().SetState(_move ? "move" : "stay");
        }
    }
    void MoveToTower(Transform target, int count)
    {       
        for (int i = 0; i < count; i++)
        {
            _hostageList[0].GetComponent<Hostage>().MoveToTower(target);
            RemoveHostage(_hostageList[0]);
        }
    }
    public void LeftStep() { } 
    public void RightStep() { }

    public void Damage()
    {       
        hostCountText.gameObject.transform.parent.gameObject.SetActive(false);
        //GameObject eff =  Instantiate(effect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        GameObject eff = PoolControll.Instance.Spawn("Effect");
        eff.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        gameObject.SetActive(false);
    }
    public void Stop()
    {
        SetAnimation("stay");
    }  
    public void MoveToTarget(Transform targ)
    {
        for (int i = 0; i < _hostageList.Count; i++)
        {
            _hostageList[i].GetComponent<Hostage>()._target = targ;
        }
    }
}