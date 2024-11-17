using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneUIManager : MonoBehaviour
{
    public TextMeshProUGUI pressInteractionMessage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPressInteractionImage()
    {
        pressInteractionMessage.enabled = true;
    }
    public void HidePressInteractionImage()
    {
        pressInteractionMessage.enabled = false;
    }
}
