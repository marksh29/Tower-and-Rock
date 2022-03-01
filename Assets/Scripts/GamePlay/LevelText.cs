using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    public Text levelText;
    void Start()
    {
        levelText.text = "LEVEL " + (PlayerPrefs.GetInt("level") +1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
