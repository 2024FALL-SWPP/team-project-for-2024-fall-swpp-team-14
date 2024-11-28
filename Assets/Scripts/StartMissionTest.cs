using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMissionTest : MonoBehaviour
{
    private RiveAnimationManager riveAnimationManager;
    public bool[] startMissionStatus = new bool[4];
    private void Awake()
    {
        riveAnimationManager = GameObject.Find("RiveAnimationManager").GetComponent<RiveAnimationManager>();
        riveAnimationManager.isMainMapMissionCleared = startMissionStatus;
    }
}
