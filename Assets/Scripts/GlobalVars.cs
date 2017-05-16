using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    public static string level = "MainScene";
    public static bool paused;
    public static float resetTimer;
    public static bool cutScene = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)&&resetTimer <= 0)
        {
            resetTimer = 0.05f;
            paused = false;
        }
        if(resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
        }
        if((Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P)) && !paused)
        {
            paused = true;
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && paused)
        {
            paused = false;
        }
    }
}
