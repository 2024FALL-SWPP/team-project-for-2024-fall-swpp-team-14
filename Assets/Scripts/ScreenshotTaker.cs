using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    public string screenshotName = "MapScreenshot";
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Press 'P' to take a screenshot
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        Camera camera = GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("Camera component missing!");
            return;
        }

        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Application.dataPath + "/" + screenshotName + ".png";
        System.IO.File.WriteAllBytes(filePath, bytes);

        Debug.Log("Screenshot saved to: " + filePath);
    }
}
