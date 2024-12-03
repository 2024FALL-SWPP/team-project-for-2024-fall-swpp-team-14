using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DroneUIManager : MonoBehaviour
{
    public TextMeshProUGUI pressInteractionMessage;
    public Button gameOverRetryButton;
    public Button gameOverMenuScreenButton;
    public Button mapClearMenuScreenButton;
    public GameObject inGameScreen;
    public GameObject gameOverScreen;
    public GameObject mapClearScreen;
    public bool hasMapClearScreenShown = false;

    // Start is called before the first frame update
    void Start()
    {
        gameOverRetryButton.onClick.AddListener(ResetCurrentScene);
        gameOverMenuScreenButton.onClick.AddListener(ShowMenuScreen);
        mapClearMenuScreenButton.onClick.AddListener(ShowMenuScreen);
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
    void ResetCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ShowMenuScreen()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    public void ShowInGameScreen()
    {
        inGameScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        mapClearScreen.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        inGameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        mapClearScreen.SetActive(false);
    }

    public void ShowMapClearScreen()
    {
        inGameScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        mapClearScreen.SetActive(true);
        hasMapClearScreenShown = true;
    }

}
