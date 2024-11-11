using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LaptopUIManager : MonoBehaviour
{
    public TextMeshProUGUI passwordInput;
    private InteractionManager interactionManager;
    // Start is called before the first frame update
    void Start()
    {
        interactionManager = GameObject.Find("InteractionManager").GetComponent<InteractionManager>();
    }

    // Update is called once per frame
    private KeyCode[] keyCodes = {
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            interactionManager.escapeInteraction();
            Debug.Log("escape pressed");
        }
        string typedText = Input.inputString;
        if (!string.IsNullOrEmpty(typedText))
        {
            // Display typed input on the UI
            passwordInput.text += typedText;
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && passwordInput.text.Length > 0)
        {
            passwordInput.text = passwordInput.text.Substring(0, passwordInput.text.Length - 1);
        }

        if (passwordInput.text == "1414")
        {
            interactionManager.InteractionSucceed();
        }
    }


}
