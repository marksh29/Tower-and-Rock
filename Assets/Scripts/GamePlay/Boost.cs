using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boost : MonoBehaviour
{
    enum BoostVariant
    {
        plus, minus, multiply, divide
    }
    [SerializeField] BoostVariant boostType;
    [SerializeField] float count;
    [SerializeField] TextMeshPro boostText;
    [SerializeField] GameObject prefab;
    [Header("----Money Controll----")]
    [SerializeField] int nideMoney;
    [SerializeField] TextMeshPro nideMoneyText;
    void Start()
    {        
        switch(boostType)
        {
            case (BoostVariant.plus):
                boostText.text = "+" + count;
                break;
            case (BoostVariant.minus):
                boostText.text = "-" + count;
                break;
            case (BoostVariant.multiply):
                boostText.text = "x" + count;
                break;
            case (BoostVariant.divide):
                boostText.text = "/" + count;
                break;
        }
        nideMoneyText.text = nideMoney.ToString();
    }       
    public void SetBoost(int cnt)
    {
        if(Controll.Instance.money >= nideMoney)
        {
            switch (boostType)
            {
                case (BoostVariant.plus):
                    Spawn(Mathf.FloorToInt(count));
                    break;
                case (BoostVariant.minus):
                    Player.Instance.RemoveHosts(Mathf.FloorToInt(count < cnt ? count : cnt));
                    Off();
                    break;
                case (BoostVariant.multiply):
                    Spawn(Mathf.FloorToInt(cnt * (count - 1)));
                    break;
                case (BoostVariant.divide):
                    int ct = Mathf.FloorToInt(cnt - (cnt / count));
                    Player.Instance.RemoveHosts(ct < cnt ? ct : cnt);
                    Off();
                    break;
            }
            Controll.Instance.AddMoney(-nideMoney);
        }        
    }

    public void SetScale(int cnt)
    {
        switch (boostType)
        {
            case (BoostVariant.plus):
                Player.Instance.SetScale((int)count + cnt);
                break;
            case (BoostVariant.minus):
                if(cnt > (int)count)
                    Player.Instance.SetScale(cnt - (int)count);
                else
                    Player.Instance.SetScale(1);
                break;
            case (BoostVariant.multiply):
                Player.Instance.SetScale(cnt * (int)count);
                break;
            case (BoostVariant.divide):
                Player.Instance.SetScale(Mathf.FloorToInt((float)cnt /count));
                break;
        }
        Off();
    }


    public void Spawn(int ct)
    {
        for (int i = 0; i < ct; i++)
        {
            //GameObject obj = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            GameObject obj = PoolControll.Instance.Spawn("Host");
            obj.transform.position = transform.position;
            Player.Instance.AddHost(obj);
        }
        Off();
    }
    void Off()
    {
        Controll.Instance.TextUp(boostText.text);
        gameObject.SetActive(false);
    }
}
