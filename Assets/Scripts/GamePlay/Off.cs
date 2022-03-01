using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Off : MonoBehaviour
{
    [SerializeField] float timer;
    private void OnEnable()
    {
        StartCoroutine(Effect_Off());
    }
    private void Start()
    {
      
    }   
    IEnumerator Effect_Off()
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
