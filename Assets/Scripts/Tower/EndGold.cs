using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGold : MonoBehaviour
{
    [SerializeField] bool win;
    [SerializeField] int  allGold, bonusGold, endGold;
    [SerializeField] Text endGoldText, bonusGoldText;
   
    private void OnEnable()
    {
        allGold = PlayerPrefs.GetInt("gold");

        endGold = win == true ? Controll.Instance.winGold : Controll.Instance.loseGold;
        endGoldText.text = endGold.ToString();

        bonusGold = Mathf.RoundToInt((float)Controll.Instance.money / (float)Controll.Instance.moneyForGold);
        bonusGoldText.text = bonusGold.ToString();

        if(win)
        {
            allGold += endGold;
            allGold += bonusGold;
        }
        else
        {
            allGold += endGold;
        }
        PlayerPrefs.SetInt("gold", allGold);
       
    }
    private void Start()
    {        
    }
    //private IEnumerator DoMove()
    //{
    //    yield return new WaitForSeconds(1);
    //    while (money > 0)
    //    {
    //        money -= moneyPerGold;
    //        if (money < 0) money = 0;
    //        gold++;
    //        PlayerPrefs.SetInt("gold", gold);
    //        moneyText.text = money.ToString();
    //        goldText.text = gold.ToString();
    //        yield return null;
    //    }
        
    //}
}
