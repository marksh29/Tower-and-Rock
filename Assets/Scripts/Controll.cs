using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Controll : MonoBehaviour
{
    public static Controll Instance;
    public string _state;
    [SerializeField] GameObject[] panels;

    public int money;
    [SerializeField] TextMeshPro hostText;
    [SerializeField] Text moneyText;
    [SerializeField] Text[] goldText;
    [SerializeField] GameObject _textUp;
    int maxHost, winHost;

    [SerializeField] Transform player;
    [SerializeField] int pl_win_count, en_win_count, win_count;
    [SerializeField] Slider winProgress;
    [SerializeField] GameObject[] towers;

    [Header("-------End Gold-------")]
    public int winGold;
    public int loseGold, moneyForGold;


    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        Set_state("Menu");
        goldText[0].text = PlayerPrefs.GetInt("gold", 0).ToString();
        goldText[1].text = PlayerPrefs.GetInt("gold", 0).ToString();

        win_count = GameObject.FindGameObjectsWithTag("Tower").Length;
        towers = GameObject.FindGameObjectsWithTag("Tower");
        money = 0;
        MoneyText();   
        
    }
    public void SetMaxHost(int id)
    {
        maxHost = id;
        if (hostText != null)
            hostText.text = winHost + "/" + maxHost;
    }

    public void Set_state(string name)
    {
        _state = name;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(panels[i].name == name ? true : false);
        }  
        
        switch(_state)
        {          
            case ("Win"):
                Player.Instance.Stop();
                break;
            case ("Lose"):

                break;
        }
    }  

    public void StartLevel()
    {
        AddWinCount();
        Set_state("Game");
        EnemyControll.Instance?.StartEnemy();
    }
    public void Next_level()
    {
        int id = Application.loadedLevel;
        print(id);
        if(id != Application.levelCount - 1)
        {
            SceneManager.LoadScene(Application.loadedLevel + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        
    }
    public void Restart()
    {
        SceneManager.LoadScene(Application.loadedLevel);
    }
    
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(1);
        Set_state("Win");
    }

    public void LoseOn()
    {
        StartCoroutine(Lose());
    }
    public IEnumerator Lose()
    {        
        yield return new WaitForSeconds(1);   
        Set_state("Lose");
    }
       
    public void AddMoney(int id)
    {        
        money += id ;
        MoneyText();
    }
    void MoneyText()
    {
        moneyText.text = money.ToString();
        PlayerPrefs.SetInt("money", money);
    }

    public void TextUp(string name)
    {
        GameObject txt = Instantiate(_textUp, _textUp.transform.parent) as GameObject;     
        txt.transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
        txt.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = name;
        txt.SetActive(true);
    } 
    
    public void AddWinCount()
    {
        pl_win_count = 0;
        en_win_count = 0;

        for (int i = 0; i < towers.Length; i++)
        {
            if(towers[i].GetComponent<Tower>()._state == Tower.towerState.Enemy)
            {
                en_win_count++;
            }
            if (towers[i].GetComponent<Tower>()._state == Tower.towerState.Player)
            {
                pl_win_count++;
            }
        }
        WinProgress();
        if (en_win_count == win_count)
        {
            Set_state("End");
            StartCoroutine(Lose());
        }
        if (pl_win_count == win_count)
        {
            Set_state("End");
            Player.Instance.Stop();
            StartCoroutine(Win());
        }
    }   
    void WinProgress()
    {
        int max = pl_win_count + en_win_count;
        winProgress.value = (((float)en_win_count * 100) / (float)max) / 100;
    }
}
