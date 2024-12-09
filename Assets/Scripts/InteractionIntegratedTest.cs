using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionIntegratedTest : MonoBehaviour
{
    public GameObject drone;
    public GameObject laptop;
    public GameObject pressInteractionImage;
    public TextMeshProUGUI pressInteractionMessage;
    public float testTime = 0f;
    bool[] isTested = { false, false, false };
    // Start is called before the first frame update
    void Start()
    {
        pressInteractionMessage = pressInteractionImage.GetComponent<TextMeshProUGUI>();
        drone.transform.position = new Vector3(0, 0, 29);
        laptop.transform.position = new Vector3(0, -0.7f, 30);
        laptop.transform.rotation = Quaternion.Euler(0, 90, 0);
        DroneController droneController = drone.GetComponent<DroneController>();
        droneController.DisableControl();
    }

    // Update is called once per frame
    void Update()
    {
        testTime += Time.deltaTime;
        laptop.transform.Translate(0, 0.2f * Time.deltaTime, 0);
        if (testTime > 2.0f && !isTested[0])
        {
            checkPressInteractionMessageEnabled(false);
            isTested[0] = true;
        }
        if (testTime > 4.0f && !isTested[1])
        {
            checkPressInteractionMessageEnabled(true);
            isTested[1] = true;
        }
        if (testTime > 6.0f && !isTested[2])
        {
            checkPressInteractionMessageEnabled(false);
            isTested[2] = true;
        }
    }

    void checkPressInteractionMessageEnabled(bool expected)
    {
        Debug.Assert(pressInteractionMessage.enabled == expected);
        Debug.Log("[checkPressInteractionMessageEnabled] expected : " + expected + ", current : " + pressInteractionMessage.enabled);
    }
}
