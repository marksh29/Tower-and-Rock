using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Transform target;
    [SerializeField] float speed;
    [SerializeField] TrailRenderer trailRend;
    [SerializeField] Material[] materials;
    bool move;
    void Start()
    {

    }
    public void SetTarget(Transform obj, int id)
    {
        trailRend.material = materials[id];
        target = obj;
        move = true;        
    }
    void Update()
    {
        if (move)
        {
            if (target != null && target.gameObject.activeSelf)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y + 1, target.position.z), speed * Time.deltaTime);
            else
                gameObject.SetActive(false);
        }        
    }
    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Host" && coll.gameObject == target.gameObject)
        {
            coll.gameObject.GetComponent<Hostage>().Destroy();           
        }
        if(coll.gameObject.tag == "Player")
        {
            coll.gameObject.GetComponent<Player>().BulletDamage();
        }
        gameObject.SetActive(false);
    }
}
