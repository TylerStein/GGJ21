using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button playOfflineButton;
    public Button quitButton;

    public int playOfflineSceneIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        playOfflineButton.onClick.AddListener(() => {
            SceneManager.LoadScene(playOfflineSceneIndex);
        }); 

        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            quitButton.gameObject.SetActive(false);
        } else {
            quitButton.onClick.AddListener(() => {
                Application.Quit();
            });
        }
    }
}
