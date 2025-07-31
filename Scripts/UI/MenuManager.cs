using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject warningPanel;
    public GameObject continueButton;
    public Text loadingText;

    private string savePlayerPath => Path.Combine(Application.persistentDataPath, "savePlayerData.json");
    private string saveMapPath => Path.Combine(Application.persistentDataPath, "saveMapData.json");
    private string originMapPath => Path.Combine(Application.streamingAssetsPath, "originMapData.json");

    private void Start()
    {
        // C:\Users\Admin\AppData\LocalLow\DefaultCompany\SignalHome
        if (!File.Exists(savePlayerPath))
        {
            continueButton.SetActive(false);
        }
    }
    public void StartGame()
    {
        if (File.Exists(savePlayerPath))
        {
            warningPanel.SetActive(true);
        }
        else
        {
            File.Copy(originMapPath, saveMapPath, true);  
            StartCoroutine(LoadGameScene());
        }
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadGameScene());
    }

    public void RestartGame(bool confirm)
    {
        if (confirm)
        {
            warningPanel.SetActive(false);
            File.Delete(savePlayerPath);
            File.Copy(originMapPath, saveMapPath, true);
            StartCoroutine(LoadGameScene());
        }
        else
        {
            warningPanel.SetActive(false);
        }
    }

    IEnumerator LoadGameScene()
    {
        loadingPanel.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            loadingText.text = $"·Îµù Áß... {progress * 100:F0}%"; 
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}