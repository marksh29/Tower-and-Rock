using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEffect : MonoBehaviour
{
    [SerializeField] GameObject effectOn, effectOff;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        GameObject obj = Instantiate(effectOn, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<ParticleSystemRenderer>().material = GetComponent<MeshRenderer>().material;
        Destroy(obj, 1);
    }
    private void OnDisable()
    {
        GameObject obj = Instantiate(effectOff, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<ParticleSystemRenderer>().material = GetComponent<MeshRenderer>().material;
        Destroy(obj, 1);
    }
}
