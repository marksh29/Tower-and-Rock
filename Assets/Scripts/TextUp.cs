using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUp : MonoBehaviour
{
    public float speed;
    public bool moveUp;
    void Start()
    {
        if(moveUp)
            Destroy(gameObject, 1);
    }    
    void Update()
    {        
        if(moveUp)
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        else
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }   
}
