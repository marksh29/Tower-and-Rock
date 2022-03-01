using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    bool move;
    [SerializeField] int _addMoney;
    [SerializeField] Transform _target;
    [SerializeField] float _dropForce, _speed;
    public Tower tower;
    private void OnEnable()
    {
        _target = null;
        GetComponent<BoxCollider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
        move = false;
       
        if (Controll.Instance._state == "Game")
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 3.5f, Random.Range(-1f, -0.1f)) * _dropForce, ForceMode.Impulse);
        }
    }
    void Start()
    {
             
    }
    void Update()
    {        
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.position.x, _target.position.y + 1, _target.position.z), _speed * Time.deltaTime);
            if (transform.position == new Vector3(_target.position.x, _target.position.y + 1, _target.position.z))
            {
                if(_target.gameObject.tag == "Player")
                {
                    if (Sound.Instance != null) Sound.Instance.Play_Sound("money_off");
                    Controll.Instance.AddMoney(20);
                }
                gameObject.SetActive(false);
            }
        }
        else
        {
            transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }
    }
    public void MoveOn(Transform target)
    {
        Off();
        _target = target;
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        move = true;
    } 
    public void Off()
    {
        tower.RemoveMoney(gameObject);
    }
}
