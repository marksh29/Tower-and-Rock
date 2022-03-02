using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tower : MonoBehaviour
{
    public enum towerState
    { 
       Player, Enemy, Neutral
    }
    public towerState _state;
    [Header("----Tower Parametrs----")]
    public int floorCount;
    public int countPerLevel, startFloor;
    public bool stickman;

    [Header("----Game----")]
    public int count;
    public int maxCount;
    [SerializeField] TextMeshPro countText;

    [SerializeField] GameObject floor, down, roof, destroyPrefab;
    [SerializeField] List<GameObject> levels;
    [SerializeField] Material[] playerMat, enemyMat, neutralMat;
    [SerializeField] SkinnedMeshRenderer towerPlayer;
    [SerializeField] LocationRecolor towerLocation;

    [Header("----Fire Controll----")]
    public TowerFire towerFire;
    public float startRadius, addRadius, fireTimer;

    [Header("----Drop Money----")]
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] float dropTimer;
    [SerializeField] int startMoneyDrop, addMoneyDrop, maxMoney;
    [SerializeField] List<GameObject> moneyList;
    float dTime;

    [Header("----------Host Spawner----------")]
    [SerializeField] float spawnTimer;
    [SerializeField] GameObject hostPrefab;
    [SerializeField] Transform spawnPosition;
    float timer;
    private void Awake()
    {
        maxCount = floorCount * countPerLevel;
        count = startFloor * countPerLevel;
        TowerGenerate();
    }
    void Start()
    {
        dTime = dropTimer;
        CountText();
        Recolor();      
    }
    void TowerGenerate()
    {
        floor.SetActive(false);
        for (int i = 0; i < floorCount; i++)
        {
            GameObject obj = Instantiate(floor, transform.position, transform.rotation, transform) as GameObject;
            obj.transform.localPosition = floorPos(i);
            levels.Add(obj);
        }
        LevelEnable();
    }
    Vector3 floorPos(int id)
    {
        Vector3 vect = new Vector3(down.transform.localPosition.x, down.transform.localPosition.y + (0.85f * (id + 1)), down.transform.localPosition.z);
        return vect;
    }
    void Update()
    {
        if(Controll.Instance._state == "Game")
        {
            if (_state == towerState.Player)
            {
                dTime -= Time.deltaTime;
                if (dTime <= 0)
                {
                    dTime = dropTimer;
                    StartCoroutine(DropMoney());
                }
            }

            if (_state == towerState.Player || _state == towerState.Enemy)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    timer = spawnTimer;
                    SpawnHost();
                }
            }
        }
    }

    void SpawnHost()
    {
        GameObject obj = PoolControll.Instance.Spawn("Host");        
        obj.transform.position = spawnPosition.position;
        if(_state == towerState.Player)
        {
            Player.Instance.AddHost(obj);
        }
        else
        {
            Enemy.Instance.AddHost(obj);
        }
    }

    public int FullCount(string name, int id)
    {
        int full = new int();
        switch (name) 
        {
            case ("Player"):
                switch(_state)
                {
                    case (towerState.Player):
                        full = id < maxCount - count ? id : maxCount - count;                       
                        break;
                    case (towerState.Enemy):
                    case (towerState.Neutral):
                        full = id < count + maxCount ? id : count + maxCount;
                        break;
                }                
                break;
            case ("Enemy"):
                switch (_state)
                {
                    case (towerState.Enemy):
                        full = id < maxCount - count ? id : maxCount - count;
                        break;
                    case (towerState.Player):
                    case (towerState.Neutral):
                        full = id;
                        break;
                }
                break;
        }
        return full;
    }

    public IEnumerator ScaleAdd(float time, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Enter("Player");               
            yield return new WaitForSeconds(time);
        }
    }
    
    public void Enter(string name)
    {
        switch(_state)
        {
            case (towerState.Neutral):
                if(count > 0)
                    RemoveCount();
                else
                {
                    if(name == "Player")
                    {
                        Controll.Instance.TextUp("NEW TOWER!");
                        _state = towerState.Player;
                    }                      
                    else
                    {
                        _state = towerState.Enemy;
                    }                        
                    Controll.Instance.AddWinCount();
                    //towerFire.SetState(_state.ToString());
                    AddCount();
                }
                break;
            case (towerState.Player):
            case (towerState.Enemy):
                if (name == _state.ToString())
                    AddCount();
                else
                    RemoveCount();
                break;
        }         
    }
    void AddCount()
    {
        count++;
        CountText();
        LevelEnable();
        
        if (count >= 1)
            Recolor();
    }
    void RemoveCount()
    {
        count--;
        CountText();
        LevelEnable();
        
        if (count == 0)
        {
            _state = towerState.Neutral;
            //towerFire.SetState(_state.ToString());
            Recolor();
        }
    }
    void CountText()
    {        
        if (count > maxCount) count = maxCount;
        countText.text = count == maxCount ? "MAX" : count.ToString();// + "/" + maxCount;
    }

    void LevelEnable()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (i < count / countPerLevel)
            {
                if (!levels[i].activeSelf)
                    levels[i].SetActive(true);
            }
            else
            {
                if (levels[i].activeSelf)
                { 
                    GameObject eff = PoolControll.Instance.Spawn("Destroy");
                    eff.transform.position = levels[i].transform.position;
                    eff.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = effectColor();
                    eff.SetActive(true);
                    levels[i].SetActive(false);
                }                
            }
        }
        SetRoof();
    }  
    void SetRoof()
    {
        roof.SetActive(true);
        int ct = Mathf.FloorToInt(count / countPerLevel);
        roof.transform.localPosition = new Vector3(down.transform.localPosition.x, down.transform.localPosition.y + (0.85f * (ct + 1)), down.transform.localPosition.z);        
    }

    Material effectColor()
    {
        Material mat = playerMat[0];       
        switch (_state)
        {
            case (towerState.Neutral):
                mat = neutralMat[0];
                break;
            case (towerState.Player):
                mat = playerMat[0];
                break;
            case (towerState.Enemy):
                mat = enemyMat[0];
                break;
        }
        return mat;
    }

    void Recolor()
    {
        switch (_state)
        {
            case (towerState.Player):
                down.GetComponent<MeshRenderer>().material = playerMat[0];
                roof.GetComponent<MeshRenderer>().material = playerMat[0];
                for (int i = 0; i < levels.Count; i++)
                {
                    Material[] materials = levels[i].GetComponent<MeshRenderer>().materials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        materials[m] = playerMat[m];
                    }                  
                    levels[i].GetComponent<MeshRenderer>().materials = materials;
                }
                countText.gameObject.SetActive(true);
                towerFire.SetFireState(true, Mathf.FloorToInt(count/ countPerLevel));

                break;
            case (towerState.Enemy):
                down.GetComponent<MeshRenderer>().material = enemyMat[0];
                roof.GetComponent<MeshRenderer>().material = enemyMat[0];
                for (int i = 0; i < levels.Count; i++)
                {
                    Material[] materials = levels[i].GetComponent<MeshRenderer>().materials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        materials[m] = enemyMat[m];
                    }
                    levels[i].GetComponent<MeshRenderer>().materials = materials;
                }
                countText.gameObject.SetActive(true);
                towerFire.SetFireState(true, Mathf.FloorToInt(count / countPerLevel));
                break;
            case (towerState.Neutral):
                down.GetComponent<MeshRenderer>().material = neutralMat[0];
                roof.GetComponent<MeshRenderer>().material = neutralMat[0];
                for (int i = 0; i < levels.Count; i++)
                {
                    Material[] materials = levels[i].GetComponent<MeshRenderer>().materials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        materials[m] = neutralMat[m];
                    }
                    levels[i].GetComponent<MeshRenderer>().materials = materials;
                }
                countText.gameObject.SetActive(false);
                towerFire.SetFireState(false, 0);
                break;
        }       
        towerFire.SetState(_state.ToString());
        towerLocation.StartRecolor(_state.ToString());
        TowerPlayer();
        countText.gameObject.SetActive(false);
    }
    void TowerPlayer()
    {
        towerPlayer.transform.parent.gameObject.SetActive(_state == towerState.Neutral ? false : stickman);
        switch (_state)
        {
            case (towerState.Neutral):
                towerPlayer.material = neutralMat[0];
                break;
            case (towerState.Player):
                towerPlayer.material = playerMat[0];
                break;
            case (towerState.Enemy):
                towerPlayer.material = enemyMat[0];
                break;
        }
    }
    IEnumerator DropMoney()
    {
        int ct = startMoneyDrop + (addMoneyDrop * Mathf.FloorToInt(count / countPerLevel));
        if (ct + moneyList.Count > maxMoney)
            ct = maxMoney - moneyList.Count;
        for (int i = 0; i < ct; i++)
        {
            GameObject obj = PoolControll.Instance.Spawn("Money");
            obj.GetComponent<Money>().tower = this;
            obj.transform.position = countText.gameObject.transform.position;
            moneyList.Add(obj);
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void RemoveMoney(GameObject obj)
    {
        moneyList.Remove(obj);
    }
}
