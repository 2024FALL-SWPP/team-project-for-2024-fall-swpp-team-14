using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMapManager : MonoBehaviour
{
    private GameObject player;
    private Transform playerPosition;
    public bool isServerActivated = false;
    private GameObject ceiling;
    private Image targetImage;
    private float blinkSpeed = 1.0f;
    private float initialActivationTime;
    private RiveAnimationManager riveAnimationManager;
    private AudioSource audioSource; // AudioSource 컴포넌트를 연결
    public AudioClip mission4Clip;
    public AudioClip plainClip;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ceiling = GameObject.Find("Ceiling Skylight");
        isServerActivated = false;
        if (DataTransfer.skiptoMainmap3)
        {
            isServerActivated = true;
        }

        GameObject obj = GameObject.Find("Alert_Red");
        targetImage = obj.GetComponent<Image>();
        Color color = targetImage.color;
        color.a = 0;
        targetImage.color = color;
        initialActivationTime = -10.0f;

        riveAnimationManager = GameObject.Find("RiveAnimationManager").GetComponent<RiveAnimationManager>();
        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isServerActivated)
        {
            foreach (Transform child in ceiling.GetComponentsInChildren<Transform>())
            {
                if (child.name == "LP_Skylight_glass_snaps")
                {
                    child.gameObject.SetActive(false);
                }
            }

            if(initialActivationTime < 0)
            {
                initialActivationTime = Time.time;
            }

            if(Time.time - initialActivationTime < 10.0f)
            {
                Color color = targetImage.color;
                color.a = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)) * 0.15f;
                targetImage.color = color;
            }
            else
            {
                Color color = targetImage.color;
                color.a = 0;
                targetImage.color = color;
            }
        }

        if(riveAnimationManager.isMainMapMissionCleared[2] && !riveAnimationManager.isMainMapMissionCleared[3])
        {
            if (audioSource.clip != mission4Clip)
            {
                audioSource.clip = mission4Clip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != plainClip)
            {
                audioSource.clip = plainClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }
}
