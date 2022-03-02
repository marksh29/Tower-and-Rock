using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public static Player Instance;

    public enum gameplayType
    {
        Line, Big
    }

    public enum joystickType
    {
        Static, Dinamic
    }
    [Header("-------Camera-------")]
    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] float camScaleAdd, camScale;    
    public gameplayType _gameplay;

    [Header("-------Joystic-------")]    
    public joystickType _joystick;
    public Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystickS;
    [SerializeField] private DynamicJoystick _joystickD;

    [Header("-------Player-------")]
    [SerializeField] private float _moveSpeed;
    public bool move;

    [Header("-------Line-------")]
    [SerializeField] int _lineWidth;
    [SerializeField] int startCount;
    [SerializeField] GameObject hostPrefab;
    public List<GameObject> _hostageList;
    [SerializeField] Transform[] _hostPos;
    [SerializeField] SkinnedMeshRenderer body;
    [SerializeField] Material[] bodyMaterial;
    Vector3 oldPositions;

    [Header("-------Big-------")]
    public int life;
    [SerializeField] float addScale, maxScale, attackTime;
    [SerializeField] Tower _targetTower;
    int scaleLife;
    bool _tow;

    [Header("-------Other-------")]
    [SerializeField] Animator anim;
    [SerializeField] TextMeshPro hostCountText;

    [SerializeField] GameObject effect;

    private void Awake()
    {
        camScale = cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;

        life = 1;
        HostText();

        _lineWidth = _lineWidth > _hostPos.Length ? _hostPos.Length : _lineWidth;
        if (Instance == null) Instance = this;
        if (_joystick == joystickType.Static)
            _joystickS.gameObject.SetActive(true);
        else
            _joystickD.gameObject.SetActive(true);
    }
    private void Start()
    {
        scaleLife = (int)((maxScale - 1) / addScale);
        SetAnimation("stay");

        for (int i = 0; i < startCount; i++)
        {
            GameObject obj = PoolControll.Instance.Spawn("Host");
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    AddHost(obj);
                    break;
                case (gameplayType.Big):
                    obj.GetComponent<Hostage>().Destroy();
                    AddScale();
                    break;
            }
        }
    }
    private void FixedUpdate()
    {
        if (Controll.Instance._state == "Game")
        {
            _rigidbody.velocity = new Vector3(Joyctick("X") * _moveSpeed, _rigidbody.velocity.y, Joyctick("Y") * _moveSpeed);
            if (Joyctick("X") >= 0.1f || Joyctick("Y") >= 0.1f || Joyctick("X") <= -0.1f || Joyctick("Y") <= -0.1f)
            {
                transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
                SetMovePosition();
            }

            if (cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance < camScale)
                cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance += camScaleAdd;
            if(cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance > camScale)
                cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance -= camScaleAdd;
        }
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

    float Joyctick(string name)
    {
        float XY = new float();
        switch (_joystick)
        {
            case (joystickType.Static):
                if (name == "X")
                    XY = _joystickS.Horizontal;
                else
                    XY = _joystickS.Vertical;
                break;
            case (joystickType.Dinamic):
                if (name == "X")
                    XY = _joystickD.Horizontal;
                else
                    XY = _joystickD.Vertical;
                break;
        }
        return XY;
    }

    //private void LateUpdate()
    //{
    //    if (_hostPos[0].position != oldPositions && _hostageList.Count > 0)
    //    {
    //        oldPositions = _hostPos[0].position;
    //        for (int i = 0; i < _hostageList.Count; i++)
    //        {
    //            if (i < _lineWidth)
    //            {
    //                _hostageList[i].GetComponent<Hostage>().StartMove(_hostPos[i]);
    //            }
    //            else
    //            {
    //                int ct = i - _lineWidth;
    //                _hostageList[i].GetComponent<Hostage>().StartMove(_hostageList[ct].transform.GetChild(0));
    //            }
    //        }
    //    }
    //}

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
        if (coll.gameObject.tag == "Hostage")
        {
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    AddHost(coll.gameObject.transform.parent.gameObject);
                    break;
                case (gameplayType.Big):
                    coll.gameObject.transform.parent.gameObject.GetComponent<Hostage>().Destroy();
                    AddScale();
                    break;
            }              
        }
        if (coll.gameObject.tag == "Tower")
        {
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    int id = coll.gameObject.GetComponent<Tower>().FullCount("Player", _hostageList.Count);
                    MoveToTower(coll.gameObject.transform, id);

                    break;
                case (gameplayType.Big):
                    _tow = true;
                    _targetTower = coll.gameObject.GetComponent<Tower>();
                    StartCoroutine(BigTowerDamage());
                    break;
            }
        }
        if (coll.gameObject.tag == "Boost")
        {
            switch (_gameplay)
            {
                case (gameplayType.Line):
                    coll.gameObject.GetComponent<Boost>().SetBoost(_hostageList.Count);
                    break;
                case (gameplayType.Big):
                    coll.gameObject.GetComponent<Boost>().SetScale(life);
                    break;
            }            
        }
        if (coll.gameObject.tag == "Money")
        {
            coll.gameObject.GetComponent<Money>().MoveOn(transform);
        }

        if (coll.gameObject.tag == "TowerFire" && _gameplay == gameplayType.Big)
        {
            coll.gameObject.GetComponent<TowerFire>().PlayerTarget(gameObject.transform);
        }
    }
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Host" && coll.gameObject.GetComponent<Hostage>()._status == Hostage.colorState.Enemy)
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
                        Controll.Instance.LoseOn();
                        Damage();
                    }
                    break;
                case (gameplayType.Big):
                    coll.gameObject.GetComponent<Hostage>().Destroy();
                    RemoveScale();
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Tower")
        {            
            switch (_gameplay)
            {               
                case (gameplayType.Big):
                    _tow = false;
                    StopCoroutine("BigTowerDamage");                    
                    break;
            }
        }
        if (coll.gameObject.tag == "TowerFire" && _gameplay == gameplayType.Big)
        {
            coll.gameObject.GetComponent<TowerFire>().RemovePlayerTarget(gameObject.transform);
        }
    }

    public void AddHost(GameObject obj)
    {
        Controll.Instance.TextUp("+1");
        _hostageList.Add(obj);
        SetMovePosition();

        obj.GetComponent<Hostage>().SetState("move");
        obj.GetComponent<Hostage>().SetStateColor("Player");
                
        HostText();
    }

    public void RemoveHosts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //Destroy(_hostageList[_hostageList.Count - 1]);
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
        SetCamScale(life);
    }
    public void RemoveScale()
    {
        life--;
        transform.localScale -= new Vector3(addScale, addScale, addScale);

        StartCoroutine(BodyColor());
        HostText();
        if (life <= 0)
        {
            Controll.Instance.LoseOn();
            Damage();
        }
        SetCamScale(life);
    }
    public void SetScale(int id)
    {
        life = id;
        float scl = 1f + (float)id * addScale;
        transform.localScale = new Vector3(scl,scl, scl);
        HostText();
        SetCamScale(life);
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
    public void LeftStep()
    {
        Sound.Instance.Play_Sound("p_step_l");
    }
    public void RightStep()
    {
        Sound.Instance.Play_Sound("p_step_r");
    }

    public void Damage()
    {       
        hostCountText.gameObject.transform.parent.gameObject.SetActive(false);
        //Instantiate(effect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        GameObject eff = PoolControll.Instance.Spawn("Effect");
        eff.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        gameObject.SetActive(false);
    }  
    public void Stop()
    {
        SetAnimation("stay");
    }   

    IEnumerator BodyColor()
    {
        body.material = bodyMaterial[0];
        yield return new WaitForSeconds(0.02f);
        body.material = bodyMaterial[1];
    }

    void SetCamScale(int life)
    {
        camScale = 50 + ((life - 1) * 0.5f);
    }

    IEnumerator BigTowerDamage()
    {
        yield return new WaitForSeconds(attackTime);
        if (life > 1 && _tow)
        {
            RemoveScale();
            _targetTower.Enter("Player");
            StartCoroutine(BigTowerDamage());
        }            
    }
    public void BulletDamage()
    {        
        if (_gameplay == gameplayType.Big)
        {
            RemoveScale();
        }
    }
}