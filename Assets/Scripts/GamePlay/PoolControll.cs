using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolControll : MonoBehaviour
{
    public static PoolControll Instance;
    [SerializeField] private GameObject host, effect, destroy_effect, bullet, money;
    [SerializeField] private List<GameObject> host_stack, effect_stack, destroy_stack, bullet_stack, money_stack;
    GameObject new_obj, obj;

    private void Start()
    {
        
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;        
    }   
    public GameObject Spawn(string name)
    {
        switch (name)
        {
            case ("Host"):
                new_obj = Spawn(host_stack, host);
                break;
            case ("Effect"):
                new_obj = Spawn(effect_stack,effect);
                break;
            case ("Destroy"):
                new_obj = Spawn(destroy_stack, destroy_effect);
                break;
            case ("Bullet"):
                new_obj = Spawn(bullet_stack, bullet);
                break;
            case ("Money"):
                new_obj = Spawn(money_stack, money);
                break;
        }
        return new_obj;       
    }
    GameObject Spawn(List<GameObject> list, GameObject prefab)
    {
        bool not_empty = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].activeSelf)
            {
                list[i].SetActive(true);
                obj = list[i];
                not_empty = true;
                break;
            }
        }
        if (not_empty == false)
        {
            GameObject new_obj = Instantiate(prefab) as GameObject;
            new_obj.SetActive(true);
            obj = new_obj;
            list.Add(new_obj);
        }
        return obj;
    }    
}
