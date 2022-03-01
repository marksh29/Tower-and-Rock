using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analytic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TinySauce.OnGameStarted();
        TinySauce.OnGameStarted(levelNumber: (Application.loadedLevel).ToString());
    }    
}
