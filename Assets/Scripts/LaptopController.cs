using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LaptopController : InteractionController
{
    public TextMeshProUGUI passwordInput;
    public string passwordAnswer = "defaultPassword";
    private bool isInteractionDoing = false;
    public GameObject enterPassword;
    public GameObject passwordGuide;
    private DroneController droneController;

    private KeyCode[] NumberkeyCodes =
    {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    void Start()
    {
        droneController = GameObject.Find("Drone").GetComponent<DroneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInteractionSuccessful && isInteractionDoing && droneController.droneGameState == DroneController.DroneGameState.InGame)
        {
            for (int i = 0; i < NumberkeyCodes.Length; i++)
            {
                if (Input.GetKeyDown(NumberkeyCodes[i]))
                {
                    if (passwordInput.text.Length < 4)
                    {
                        passwordInput.text += "" + i;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                interactionManager.ExitInteraction(false);
                Debug.Log("escape pressed");
            }
            if (Input.GetKeyDown(KeyCode.Backspace) && passwordInput.text.Length > 0)
            {
                passwordInput.text = passwordInput.text.Substring(0, passwordInput.text.Length - 1);
            }

            if (passwordInput.text == passwordAnswer)
            {
                isInteractionSuccessful = true;
                isInteractionDoing = false;
                interactionManager.ExitInteraction(true);
                Debug.Log("Laptop interaction succeed");
                passwordInput.text = "SUCCESS";
                passwordInput.fontSize = 15;
                passwordInput.color = Color.green;
                enterPassword.SetActive(false);
                passwordGuide.SetActive(false);
            }
        }
    }

    public override void StartInteraction()
    {
        isInteractionDoing = true;
    }

}
