using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LocationRecolor : MonoBehaviour
{
    [SerializeField] GameObject[] box;
    //[SerializeField] MeshRenderer[] box;
    [SerializeField] float recolorTime;

    [SerializeField] Material player, neitral, enemy;
    Material curColor;
    
    void Start()
    {
        box = box.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();
        foreach (GameObject point in box);
    }
    public void StartRecolor(string name)
    {
        switch(name)
        {
            case ("Enemy"):
                curColor = enemy;
                break;
            case ("Neutral"):
                curColor = neitral;
                break;
            case ("Player"):
                curColor = player;
                break;
        }

        if(Controll.Instance._state == "Game")
        {
            StartCoroutine(Recolor());
        }
        else
        {
            for (int i = 0; i < box.Length; i++)
            {
                box[i].GetComponent<MeshRenderer>().sharedMaterial = curColor;
            }
        }
    }
    IEnumerator Recolor()
    {
        for (int i = 0; i < box.Length; i++)
        {
            box[i].GetComponent<MeshRenderer>().sharedMaterial = curColor;
            yield return new WaitForSeconds(recolorTime);
        }        
    }
}
