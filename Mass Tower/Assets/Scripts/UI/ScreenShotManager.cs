using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScreenShotManager : MonoBehaviour
{
    public GameObject showImagePanel;
    public GameObject capturePanel;
    public GameObject saveImagePanel;
    public GameObject Ads1;
    public GameObject Ads2;
    public GameObject Ads3;
    public GameObject Ads4;
    public GameObject Ads5;


    public RawImage showImage;

    public string gameName = "Mass Tower";

    private byte[] currentTexture;
    private string currentFilePath;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string ScreenshotName()
    {
        return string.Format("{0}_{1}.png", gameName, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-sss"));
    }

    public void Capture()
    {
        StartCoroutine(TakeScreenShot());
    }

    private IEnumerator TakeScreenShot()
    {
        capturePanel.SetActive(false);
        Ads1.SetActive(false);
        Ads2.SetActive(false);
        Ads3.SetActive(false);
        Ads4.SetActive(false);
        Ads5.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
        screenshot.Apply();

        currentFilePath = Path.Combine(Application.temporaryCachePath, "temp_img.png");
        currentTexture = screenshot.EncodeToPNG();
        
        File.WriteAllBytes(currentFilePath,currentTexture);
        ShowScreenshot();
        capturePanel.SetActive(true);
        Ads1.SetActive(true);
        Ads2.SetActive(true);
        Ads3.SetActive(true);
        Ads4.SetActive(true);
        Ads5.SetActive(true);
        
        //
        //
        // To avoid memory leaks
        Destroy(screenshot);
        
        
    }

    public void ShowScreenshot()
    {
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.LoadImage(currentTexture);
        showImage.material.mainTexture = tex;
        showImagePanel.SetActive(true);
    }

    public void SavetoGallery()
    {
        NativeGallery.Permission permission =
            NativeGallery.SaveImageToGallery(currentFilePath, gameName, ScreenshotName(),
                (success, path) =>
                {
                    Debug.Log("Media Save Result" + success + " " + path);
                    if (success)
                    {
                        saveImagePanel.SetActive(true);
#if UNITY_EDITOR
                        string editorFilePath = Path.Combine(Application.persistentDataPath, ScreenshotName());
                        File.WriteAllBytes(editorFilePath, currentTexture);
#endif                        
                    }
                });
        Debug.Log("Permission result" + permission);
        
    }

    public void ShareImage()
    {
        new NativeShare().AddFile(currentFilePath)
            .SetSubject("Share Screenshot Mass Tower").SetText("Hello World")
            .SetCallback((result, shareTarget) => 
        Debug.Log("Share result: "+ result+", selected app: "+shareTarget)).Share();
    }
    
}
