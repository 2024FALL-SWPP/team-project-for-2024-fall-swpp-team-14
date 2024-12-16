using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIMusicIntegratedTestMenu : MonoBehaviour
{
    private float StartTime;
    private RiveScreenMenu riveScreenMenu;

    void Start()
    {
        StartTime = Time.time;
        riveScreenMenu = GameObject.Find("Main Camera").GetComponent<RiveScreenMenu>();
    }

    void Update()
    {
        if (Time.time - StartTime > 5.0f)
        {
            riveScreenMenu.landPosition = new Vector3(42, 11, -13);
            riveScreenMenu.landRotation = Quaternion.Euler(0, 130, 0);
            DataTransfer.skiptoMainmap3 = true;
            SceneManager.LoadSceneAsync("Map_v2", LoadSceneMode.Single).completed += riveScreenMenu.OnSceneLoaded;
        }
    }
}
