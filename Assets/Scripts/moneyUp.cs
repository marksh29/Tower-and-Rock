using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyUp : MonoBehaviour
{
    public float speed, stopTime, destroyTime;
    bool moveUp;
    void Start()
    {
        StartCoroutine(Up());
    }

    // Update is called once per frame
    void Update()
    {
        if (moveUp)
            transform.Translate(Vector3.up * speed * Time.deltaTime);        
    }
    IEnumerator Up()
    {
        moveUp = true;
        yield return new WaitForSeconds(stopTime);
        moveUp = false;
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
